﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable once CheckNamespace
namespace SmartClass.Common.ScopeHubs
{
    public interface ISignalREvent : IHaveBags
    {
        /// <summary>
        /// 触发事件的时间
        /// </summary>
        DateTime RaiseAt { get; }
    }

    public interface IHubEvent : ISignalREvent
    {
        //hub内部使用
        Hub RaiseHub { get; }
    }

    public interface IHubContextEvent : ISignalREvent
    {
        //hub外部使用
        HubContextWrapper Context { get; }
    }

    #region HubContext Wrapper

    public class HubContextWrapper
    {
        public IHubClients<IClientProxy> Clients { get; set; }
        public IGroupManager Groups { get; set; }
    }

    public class HubContextWrapper<THub> : HubContextWrapper where THub : Hub
    {
        public IHubContext<THub> HubContext { get; set; }
    }

    public static class HubContextExtensions
    {
        public static HubContextWrapper<THub> AsHubContextWrapper<THub>(this IHubContext<THub> context) where THub : Hub
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hubContext = new HubContextWrapper<THub>();
            hubContext.Clients = context.Clients;
            hubContext.Groups = context.Groups;
            hubContext.HubContext = context;
            return hubContext;
        }
    }

    #endregion

    public abstract class SignalREvent : IHubEvent, IHubContextEvent
    {
        #region ctors

        protected SignalREvent(Hub raiseHub)
        {
            RaiseHub = raiseHub ?? throw new ArgumentNullException(nameof(raiseHub));
            RaiseAt = DateHelper.Instance.GetDateNow();
            var callingContext = raiseHub.GetSignalREventContext();
            //SendArgs = SendArgs.Create().WithSendFrom(callingContext);
        }

        protected SignalREvent(HubContextWrapper context, SendArgs sendArgs)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            //SendArgs = sendArgs ?? throw new ArgumentNullException(nameof(sendArgs));
            RaiseAt = DateHelper.Instance.GetDateNow();
        }

        #endregion

        public IDictionary<string, object> Bags { get; set; }
        public DateTime RaiseAt { get; }
        public Hub RaiseHub { get; }
        public HubContextWrapper Context { get; }
        public SendArgs SendArgs { get; set; }
        public bool StopSend { get; set; } //todo: rename

        public bool IsCalledFromHub()
        {
            return RaiseHub != null;
        }
        public bool IsCalledOutsideHub()
        {
            return !IsCalledFromHub();
        }
        public IHubClients<IClientProxy> TryGetHubClients()
        {
            if (RaiseHub != null)
            {
                return RaiseHub.Clients;
            }
            return Context.Clients;
        }
    }

    public class SendArgs
    {
        public SendFromScopeArgs SendFrom { get; set; } = new SendFromScopeArgs();
        public SendToScopeArgs SendTo { get; set; } = new SendToScopeArgs();

        public SendArgs WithSendFrom(IScopeClientLocate locate)
        {
            this.SendFrom.CopyFrom(locate);
            return this;
        }
        public static SendArgs Create()
        {
            return new SendArgs();
        }
        public static SendArgs CreateForScopeGroupAll(string scopeId)
        {
            var sendArgs = new SendArgs();
            sendArgs.SendTo = SendToScopeArgs.CreateForScopeGroupAll(scopeId);
            return sendArgs;
        }
    }

    public class SendToScopeArgs : IScopeKey
    {
        public string ScopeId { get; set; }
        public IList<string> ClientIds { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();

        public static SendToScopeArgs CreateForScopeGroupAll(string scopeId)
        {
            var sendToScopeArgs = new SendToScopeArgs();
            sendToScopeArgs.WithScopeId(scopeId);
            sendToScopeArgs.Groups.Add(HubConst.GroupName_All);
            return sendToScopeArgs;
        }
    }

    public class SendFromScopeArgs : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
    }
}

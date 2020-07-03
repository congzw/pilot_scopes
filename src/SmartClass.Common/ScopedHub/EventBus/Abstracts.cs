using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopedHub.EventBus
{
    public interface ISignalREvent : IScopeKey
    {
        /// <summary>
        /// 触发事件的时间
        /// </summary>
        DateTime RaiseAt { get; }

        /// <summary>
        /// 时间的上下文
        /// </summary>
        HubEventHandleContext HandleContext { get; set; }
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

    #region base events
    
    public abstract class BaseHubEvent : IHubEvent
    {
        protected BaseHubEvent(Hub raiseHub, string scopeId)
        {
            ScopeId = scopeId;
            RaiseAt = DateHelper.Instance.GetDateNow();
            RaiseHub = raiseHub;
        }

        public DateTime RaiseAt { get; private set; }
        public HubEventHandleContext HandleContext { get; set; } = new HubEventHandleContext();
        public Hub RaiseHub { get; private set; }
        public string ScopeId { get; set; }
    }

    public abstract class BaseHubCrossEvent : IHubEvent, IHubContextEvent
    {
        protected BaseHubCrossEvent(Hub raiseHub, string scopeId)
        {
            ScopeId = scopeId;
            RaiseAt = DateHelper.Instance.GetDateNow();
            RaiseHub = raiseHub;
        }
        protected BaseHubCrossEvent(HubContextWrapper context, string scopeId)
        {
            ScopeId = scopeId;
            RaiseAt = DateHelper.Instance.GetDateNow();
            Context = context;
        }

        public string ScopeId { get; set; }
        public DateTime RaiseAt { get; }
        public HubEventHandleContext HandleContext { get; set; } = new HubEventHandleContext();

        public Hub RaiseHub { get; }
        public HubContextWrapper Context { get; }
        public IHubClients<IClientProxy> TryGetHubCallerClients()
        {
            if (RaiseHub != null)
            {
                return RaiseHub.Clients;
            }
            return Context.Clients;
        }
    }

    #endregion

    #region handle context

    public class HubEventHandleContext : IHaveBags
    {
        public HubEventHandleContext()
        {
            Bags = BagsHelper.Create();
        }

        public IDictionary<string, object> Bags { get; set; }
    }

    public static class HubEventHandleContextExtensions
    {
        public static string ShouldWaitComplete = "ShouldWaitComplete";
        public static bool GetShouldWaitComplete(this HubEventHandleContext ctx, ISignalREventHandler handler, bool defaultValue)
        {
            return ctx.GetAs(handler, ShouldWaitComplete, defaultValue);
        }
        public static void SetShouldWaitComplete(this HubEventHandleContext ctx, ISignalREventHandler handler, bool shouldWait)
        {
            ctx.Set(handler, ShouldWaitComplete, shouldWait);
        }

        public static string ShouldThrowWhenException = "ShouldThrowWhenException";
        public static bool GetShouldThrowWhenException(this HubEventHandleContext ctx, ISignalREventHandler handler, bool defaultValue)
        {
            return ctx.GetAs(handler, ShouldThrowWhenException, defaultValue);
        }
        public static void SetShouldThrowWhenException(this HubEventHandleContext ctx, ISignalREventHandler handler, bool shouldThrow)
        {
            ctx.Set(handler, ShouldThrowWhenException, shouldThrow);
        }

        public static string ShouldStopSend = "StopSend";
        public static bool GetStopSend(this HubEventHandleContext ctx, ISignalREventHandler handler, bool defaultValue)
        {
            return ctx.GetAs(handler, ShouldStopSend, defaultValue);
        }
        public static void SetStopSend(this HubEventHandleContext ctx, ISignalREventHandler handler, bool shouldStop)
        {
            ctx.Set(handler, ShouldStopSend, shouldStop);
        }

        //helpers
        private static void Set(this HubEventHandleContext ctx, ISignalREventHandler handler, string key, object value)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            var theKey = string.Format("{0}_{1}", key, handler.GetType().FullName);
            ctx.Bags[theKey] = value;
        }
        private static object Get(this HubEventHandleContext ctx, ISignalREventHandler handler, string key, object defaultValue)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var theKey = string.Format("{0}_{1}", key, handler.GetType().FullName);
            if (!ctx.Bags.ContainsKey(theKey))
            {
                return defaultValue;
            }
            return ctx.Bags[theKey];
        }
        private static T GetAs<T>(this HubEventHandleContext ctx, ISignalREventHandler handler, string key, T defaultValue)
        {
            var theValue = ctx.Get(handler, key, defaultValue);
            if (theValue == null)
            {
                return defaultValue;
            }
            return (T)Convert.ChangeType(theValue, typeof(T));
        }
    }
    
    #endregion

    public interface ISignalREventHandler
    {
        float HandleOrder { set; get; }
        bool ShouldHandle(ISignalREvent @event);
        Task HandleAsync(ISignalREvent @event);
    }
}
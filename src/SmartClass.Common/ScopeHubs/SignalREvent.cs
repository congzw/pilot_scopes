using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

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

    public interface IHubContextWrapperHold
    {
        HubContextWrapper Wrapper { get; set; }
    }

    public class HubContextWrapper
    {
        public IHubClients<IClientProxy> Clients { get; set; }
        public IGroupManager Groups { get; set; }

        public object HubContext { get; set; }
        public IHubContext<THub> GetHubContext<THub>() where THub : Hub
        {
            return HubContext as IHubContext<THub>;
        }
    }
    
    public static class HubContextExtensions
    {
        public static HubContextWrapper AsHubContextWrapper<THub>(this IHubContext<THub> context) where THub : Hub
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var hubContext = new HubContextWrapper();
            hubContext.Clients = context.Clients;
            hubContext.Groups = context.Groups;
            hubContext.HubContext = context;
            return hubContext;
        }


        //public static HubContextWrapper<THub> AsHubContextWrapper<THub>(this IHubContext<THub> context) where THub : Hub
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException(nameof(context));
        //    }
        //    var hubContext = new HubContextWrapper<THub>();
        //    hubContext.Clients = context.Clients;
        //    hubContext.Groups = context.Groups;
        //    hubContext.HubContext = context;
        //    return hubContext;
        //}
    }

    #endregion

    public abstract class SignalREvent : IHubEvent, IHubContextEvent
    {
        #region ctors

        protected SignalREvent(Hub raiseHub, SendContext sendContext = null)
        {
            RaiseHub = raiseHub ?? throw new ArgumentNullException(nameof(raiseHub));
            SendContext = sendContext ?? raiseHub.GetSendFrom().GetSendContext();
            SendContext.SendConnectionId = raiseHub.Context.ConnectionId;

            RaiseAt = DateHelper.Instance.GetDateNow();
        }

        protected SignalREvent(HubContextWrapper hubContextWrapper, SendContext sendContext)
        {
            Context = hubContextWrapper ?? throw new ArgumentNullException(nameof(hubContextWrapper));
            SendContext = sendContext ?? throw new ArgumentNullException(nameof(sendContext));
            RaiseAt = DateHelper.Instance.GetDateNow();
        }

        #endregion
        
        public Hub RaiseHub { get; }
        public HubContextWrapper Context { get; }
        public SendContext SendContext { get; set; }
        public DateTime RaiseAt { get; }
        public bool StopSend { get; set; } //todo: rename

        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();


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
        public IGroupManager TryGetGroupManager()
        {
            if (RaiseHub != null)
            {
                return RaiseHub.Groups;
            }
            return Context.Groups;
        }
    }

    public interface ISignalREventHandler : IMyScoped
    {
        float HandleOrder { set; get; }
        bool ShouldHandle(ISignalREvent @event);
        Task HandleAsync(ISignalREvent @event);
    }
    public interface ISignalREventDispatcher : IMyTransient
    {
        Task Dispatch(ISignalREvent hubEvent);
    }

    public class SignalREventDispatcher : ISignalREventDispatcher
    {
        public IEnumerable<ISignalREventHandler> Handlers { get; }

        public SignalREventDispatcher(IEnumerable<ISignalREventHandler> signalREventHandlers)
        {
            Handlers = signalREventHandlers ?? Enumerable.Empty<ISignalREventHandler>();
        }

        public async Task Dispatch(ISignalREvent hubEvent)
        {
            var sortedHandlers = Handlers
                .Where(x => x.ShouldHandle(hubEvent))
                .OrderBy(x => x.HandleOrder)
                .ToList();

            foreach (var handler in sortedHandlers)
            {
                await handler.HandleAsync(hubEvent).ConfigureAwait(false);
            }
        }
    }
}

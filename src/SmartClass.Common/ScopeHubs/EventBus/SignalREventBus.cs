using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;

// ReSharper disable once CheckNamespace
namespace SmartClass.Common.ScopeHubs
{
    public interface ISignalREventDispatcher
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
                //todo ex handing
                await handler.HandleAsync(hubEvent).ConfigureAwait(false);
            }
        }
    }

    public class SignalREventBus
    {
        private readonly ISignalREventDispatcher _dispatcher;
        private readonly IClientConnectionRepository _connectionRepository;
        private readonly IScopeClientGroupRepository _scopeClientGroupRepository;

        public SignalREventBus(ISignalREventDispatcher dispatcher, IClientConnectionRepository connectionRepository, IScopeClientGroupRepository scopeClientGroupRepository)
        {
            _dispatcher = dispatcher;
            _connectionRepository = connectionRepository;
            _scopeClientGroupRepository = scopeClientGroupRepository;
        }

        public async Task Raise(ISignalREvent @event)
        {
            var monitorHelper = ManageMonitorHelper.Instance;

            var theEvent = (SignalREvent)@event;
            await TraceAsync(theEvent, " handling").ConfigureAwait(false);
            await _dispatcher.Dispatch(@event).ConfigureAwait(false);
            await TraceAsync(theEvent, " handled").ConfigureAwait(false);

            if (monitorHelper.Config.UpdateConnectionsEnabled)
            {
                var hubClients = theEvent.TryGetHubClients();
                var connections = _connectionRepository.GetConnections(new GetConnectionsArgs());
                var updateConnectionsArgs = UpdateConnectionsArgs.Create(connections);
                await monitorHelper.UpdateConnections(hubClients, updateConnectionsArgs);

                var scopeClientGroups = _scopeClientGroupRepository.GetScopeClientGroups(new ScopeClientGroupLocate());
                var updateClientTreeArgs = UpdateClientTreeArgs.Create(scopeClientGroups, connections);
                await monitorHelper.UpdateClientTree(hubClients, updateClientTreeArgs);
            }
        }

        private Task TraceAsync(SignalREvent theEvent, string descAppend)
        {
            var hubClients = theEvent.TryGetHubClients();
            var eventName = theEvent.GetType().Name;
            var info = new EventInvokeInfo();
            info.SendContext = theEvent.SendContext;
            info.Desc = eventName + descAppend;
            info.ConnectionId = theEvent.RaiseHub?.Context?.ConnectionId;

            return ManageMonitorHelper.Instance.EventInvoked(hubClients, info);
        }
    }
}

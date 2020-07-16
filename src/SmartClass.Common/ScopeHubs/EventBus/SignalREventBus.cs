using System;
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
            var manageMonitorHelper = ManageMonitorHelper.Instance;
            var theEvent = (SignalREvent)@event;
            try
            {
                await _dispatcher.Dispatch(@event);
                await manageMonitorHelper.TraceSignalREvent(theEvent, " handled").ConfigureAwait(false);
                await manageMonitorHelper.UpdateMonitor(theEvent, _connectionRepository, _scopeClientGroupRepository);
            }
            catch (Exception ex)
            {
                await manageMonitorHelper.TraceSignalREvent(theEvent, " !ex").ConfigureAwait(false);
                await manageMonitorHelper.ServerLog(theEvent.TryGetHubClients(), new ServerLogInfo() {Category = theEvent.GetType().Name, Message = ex.Message});
            }
        }
    }
}

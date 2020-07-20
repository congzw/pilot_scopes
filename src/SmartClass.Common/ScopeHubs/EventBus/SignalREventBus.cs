using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
}

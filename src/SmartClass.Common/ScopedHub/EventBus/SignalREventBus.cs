using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopedHub.EventBus
{
    public class SignalREventBus
    {
        public IEnumerable<ISignalREventHandler> Handlers { get; }

        public SignalREventBus(IEnumerable<ISignalREventHandler> signalREventHandlers)
        {
            Handlers = signalREventHandlers ?? Enumerable.Empty<ISignalREventHandler>();
        }
        
        public async Task Raise(ISignalREvent hubEvent)
        {
            Trace(hubEvent, "HubEvent -> ");
            if (hubEvent.HandleContext == null)
            {
                //auto fix null if not impl from base event!
                hubEvent.HandleContext = new HubEventHandleContext();
            }

            var sortedHandlers = Handlers
                .Where(x => x.ShouldHandle(hubEvent))
                .OrderBy(x => x.HandleOrder)
                .ToList();

            var ctx = hubEvent.HandleContext;

            foreach (var handler in sortedHandlers)
            {
                var shouldThrowWhenException = ctx.GetShouldThrowWhenException(handler, true);
                var shouldWaitComplete = ctx.GetShouldWaitComplete(handler, true);
                try
                {
                    Trace(handler.GetType().Name, "HubEventHandler Invoking -> ");
                    if (shouldWaitComplete)
                    {
                        await handler.HandleAsync(hubEvent).ConfigureAwait(false);
                        Trace(handler.GetType().Name, "HubEventHandler Invoked -> ");
                    }
                    else
                    {
#pragma warning disable 4014
                        handler.HandleAsync(hubEvent);
#pragma warning restore 4014
                    }
                }
                catch (Exception e)
                {
                    Trace(e, "HubEventHandler Ex -> " + handler.GetType().Name);
                    if (shouldThrowWhenException)
                    {
                        throw;
                    }
                }
            }
        }

        public static Action<string> TraceFunc { get; set; }
        
        private void Trace(object message, string preFix = null)
        {
            if (message is string)
            {
                TraceFunc?.Invoke(preFix + message);
            }
            else
            {
                TraceFunc?.Invoke(preFix + message);
            }
        }
    }
}

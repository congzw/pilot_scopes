using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common.DependencyInjection;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

// ReSharper disable CheckNamespace

namespace Common.SignalR.Scoped
{
    public static class ScopedExtensions
    {
        public static IServiceCollection AddClientMonitors(this IServiceCollection services)
        {
            services.AddScoped<SignalREventBus>();
            services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped, typeof(SignalREventHandlerDecorator));
            
            services.AddScoped<ClientInvokeProcessBus>();
            services.AddAllImpl<IClientInvokeProcess>(ServiceLifetime.Scoped);
            
            services.AddScoped<ClientStubProcessBus>();
            services.AddAllImpl<IClientStubProcess>(ServiceLifetime.Scoped);

            //todo by config
            //var traceEventBus = false;
            //if (traceEventBus)
            //{
            //    services.Decorate<ISignalREventHandler, SignalREventHandlerDecorator>();
            //}

            services.AddSingleton<IClientConnectionRepository, ClientConnectionRepository>();
            services.AddSingleton<IClientMonitor, ClientMonitor>();
            return services;
        }
    }

    public class SignalREventHandlerDecorator : ISignalREventHandler
    {
        private readonly ISignalREventHandler _signalREventHandler;

        public SignalREventHandlerDecorator(ISignalREventHandler signalREventHandler)
        {
            _signalREventHandler = signalREventHandler;
        }

        public float HandleOrder
        {
            get => _signalREventHandler.HandleOrder;
            set => _signalREventHandler.HandleOrder = value;
        }

        public bool ShouldHandle(ISignalREvent @event)
        {
            return _signalREventHandler.ShouldHandle(@event);
        }

        public async Task HandleAsync(ISignalREvent @event)
        {
            //todo trace
            await _signalREventHandler.HandleAsync(@event).ConfigureAwait(false);
        }
    }
}

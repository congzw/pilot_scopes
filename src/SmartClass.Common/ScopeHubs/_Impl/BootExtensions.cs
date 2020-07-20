using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common.DependencyInjection;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;

namespace SmartClass.Common.ScopeHubs._Impl
{
    public static class BootExtensions
    {
        public static IServiceCollection AddClientMonitors(this IServiceCollection services)
        {
            services.AddTransient<ISignalREventDispatcher, SignalREventDispatcher>();
            services.AddScoped<SignalREventBus>();
            //todo by config
            //todo: 不用第三方的DI工具，如何支持Decorator?
            //var traceEventBus = false;
            //if (traceEventBus)
            //{
            //    services.Decorate<ISignalREventHandler, SignalREventHandlerDecorator>();
            //}
            //services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped, typeof(SignalREventHandlerDecorator));
            services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped);

            services.AddScoped<ClientMethodProcessBus>();
            services.AddAllImpl<IClientMethodProcess>(ServiceLifetime.Scoped);

            services.AddSingleton<IEventLogHelper, HubClientLogHelper>();
            services.AddSingleton<ScopeClientConnectionKeyMaps>();
            services.AddSingleton<HubCallerContextCache>();
            services.AddSingleton<SignalRConnectionCache>();
            services.AddScoped<IClientMonitor, ClientMonitor>();
            return services;
        }

        public static IApplicationBuilder UseClientMonitors(this IApplicationBuilder sp)
        {
            EventLogHelper.Resolve = () => sp.ApplicationServices.GetRequiredService<IEventLogHelper>();
            return sp;
        }
    }
}

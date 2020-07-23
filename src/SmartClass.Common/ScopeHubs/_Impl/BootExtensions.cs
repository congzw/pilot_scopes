using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common.DependencyInjection;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.Scopes;

namespace SmartClass.Common.ScopeHubs._Impl
{
    public static class BootExtensions
    {
        public static IServiceCollection AddClientMonitors(this IServiceCollection services)
        {
            //already auto registered by IMyLifetime!
            //services.AddTransient<ISignalREventDispatcher, SignalREventDispatcher>();
            //services.AddScoped<SignalREventBus>();
            //services.AddAllImpl<ISignalREventHandler>(ServiceLifetime.Scoped);

            //services.AddScoped<ClientMethodProcessBus>();
            //services.AddAllImpl<IClientMethodProcess>(ServiceLifetime.Scoped);

            //services.AddSingleton<IEventLogHelper, HubClientLogHelper>();
            //services.AddSingleton<ScopeClientConnectionKeyMaps>();
            //services.AddSingleton<HubCallerContextCache>();
            //services.AddSingleton<SignalRConnectionCache>();
            //services.AddScoped<IClientMonitor, ClientMonitor>();

            services.AddSingleton<IClientContextService, RequestClientContextService>();
            services.AddSingleton(ManageMonitorHelper.Instance);

            return services;
        }

        public static IApplicationBuilder UseClientMonitors(this IApplicationBuilder app)
        {
            EventLogHelper.Resolve = () => app.ApplicationServices.GetRequiredService<IEventLogHelper>();
            ClientContext.Resolve = () => app.ApplicationServices.GetRequiredService<IClientContextService>();
            ScopeContext.ResolveScopeId = () => ClientContext.GetCurrentClientContext(null)?.ScopeId;
            return app;
        }
    }
}

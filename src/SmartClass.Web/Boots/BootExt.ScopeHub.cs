using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs._Impl;
using SmartClass.DAL;

namespace SmartClass.Web.Boots
{
    public static partial class BootExt
    {
        public static IServiceCollection AddClientMonitorsWrap(this IServiceCollection services)
        {
            services.AddClientMonitors();

            services.AddScoped<IHubContextWrapperHold, _AnyHubContextWrapperHold>();
            services.AddScoped<IClientConnectionRepository, ClientConnectionRepository>();
            services.AddScoped<IScopeClientGroupRepository, ScopeClientGroupRepository>();
            return services;
        }

        public static IApplicationBuilder UseClientMonitorsWrap(this IApplicationBuilder app)
        {
            app.UseClientMonitors();
            
            ////pipeline demo for hub
            //app.Use(async (context, next) =>
            //{
            //    //var hubContext = context.RequestServices.GetRequiredService<IHubContext<_AnyHub>>();
            //    //[13204] [_AnyHub] Pipeline >>>>>>>> /Api/Test/Raise
            //    //[10556] [_AnyHub] Pipeline >>>>>>>> /DemoHub?scopeId=s1&clientId=c1&id=UlsKEWRNiIq8YR_wV7fcPg => only first connect!!!
            //    Trace.WriteLine(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "Pipeline", context.Request.GetEncodedPathAndQuery()));
            //    if (next != null)
            //    {
            //        await next.Invoke();
            //    }
            //});

            app.UseSignalR(routes =>
            {
                routes.MapHub<_AnyHub>("/DemoHub");
            });
            return app;
        }
    }
}

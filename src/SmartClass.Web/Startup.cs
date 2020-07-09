using System.Collections.Generic;
using Common.SignalR.Scoped;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common.ScopeHubs.ClientMonitors.Applications;

namespace SmartClass.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddClientMonitors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var fileServerOptions = new FileServerOptions();
            var defaultPages = new List<string>();
            defaultPages.Add("ClientMonitors/demo/index.html");
            fileServerOptions.DefaultFilesOptions.DefaultFileNames = defaultPages;

            app.UseFileServer(fileServerOptions);
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

            app.UseMvc();
        }
    }
}

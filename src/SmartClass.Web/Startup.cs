using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Common;
using SmartClass.Web.Boots;

namespace SmartClass.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var myLifetimeRegistry = MyLifetimeRegistry.Instance;
            var assemblies = new List<Assembly>();
            assemblies.Add(typeof(MyLifetimeRegistry).Assembly);
            assemblies.Add(this.GetType().Assembly);
            myLifetimeRegistry.AutoRegister(services, assemblies);

            services.AddSingleton(MyLifetimeRegistry.Instance);

            services.AddMyServiceLocator();
            services.AddMvc();
            services.AddSignalR();
            services.AddMyDAL();
            services.AddClientMonitorsWrap();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var fileServerOptions = new FileServerOptions();
            var defaultPages = new List<string>();
            defaultPages.Add("ClientMonitors/manage/mock_monitor.html");
            fileServerOptions.DefaultFilesOptions.DefaultFileNames = defaultPages;

            app.UseFileServer(fileServerOptions);

            app.UseMyServiceLocator();
            app.UseMyDAL(env);
            app.UseClientMonitorsWrap();

            app.UseMvc();
        }
    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Web.Boots;
using SmartClass.Web.Hubs;

namespace SmartClass.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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

            app.UseMyDAL(env);
            app.UseClientMonitorsWrap();

            app.UseMvc();
        }
    }
}

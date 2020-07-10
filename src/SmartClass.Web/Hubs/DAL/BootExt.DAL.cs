using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SmartClass.Common.Data;
using SmartClass.Common.Data.Providers.EF;
using SmartClass.DAL;
using SmartClass.Domains;

// ReSharper disable once CheckNamespace
namespace SmartClass.Web.Boots
{
    public static partial class BootExt
    {
        public static IServiceCollection AddMyDAL(this IServiceCollection services)
        {
            //for hbl temp repos
            services.AddDbContext<HblTempDbContext>(options =>
            {
                //options.UseSqlite("Data Source=HblTempDb.db");
                options.UseInMemoryDatabase("Data Source=HblTempDb.db");
                //options.UseLoggerFactory(loggerFactory);
            });
            services.AddScoped<IHblTempRepository>(sp =>
            {
                var hblDbContext = sp.GetService<HblTempDbContext>();
                var efRepository = new EFRepository(hblDbContext);
                return new HblRepositoryAdapter(efRepository);
            });

            //默认返回持久化存储
            services.AddScoped<ISimpleRepository>(sp => sp.GetService<IHblDbRepository>());
            return services;
        }
        
        public static void UseMyDAL(this IApplicationBuilder app, IHostingEnvironment hostingEnv)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                //临时数据库
                var hblTempDbContext = scope.ServiceProvider.GetService<HblTempDbContext>();
                hblTempDbContext.Database.EnsureDeleted();
                hblTempDbContext.Database.EnsureCreated();
                
                ////持久数据库 
                //var hblDbContext = scope.ServiceProvider.GetService<HblDbContext>();
                //if (!hostingEnv.IsProduction())
                //{
                //    //产品环境，不应该删除
                //    hblDbContext.Database.EnsureDeleted();
                //}
                //hblDbContext.Database.EnsureCreated();
            }
        }
    }
}

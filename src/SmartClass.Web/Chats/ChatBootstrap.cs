using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartClass.Web.Chats.AppServices;

namespace SmartClass.Web.Chats
{
    public static class ChatBootstrap
    {
        public static IServiceCollection AddChats(this IServiceCollection services)
        {
            services.AddDbContext<ChatDbContext>(options =>
            {
                //options.UseInMemoryDatabase("Data Source=ChatDb.db");
                options.UseSqlite("Data Source=ChatDb.db");
            });

            services.AddScoped<ISeedAppService, SeedAppService>();
            services.AddScoped<IRoomAppService, RoomAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            return services;
        }

        public static IApplicationBuilder UseChats(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var seedAppService = scope.ServiceProvider.GetService<ISeedAppService>();
                var seedArgs = new SeedArgs() { DeleteExist = false, RoomCount = 3, UserCount = 10 };
                seedAppService.Seed(seedArgs);
            }
            return app;
        }
    }
}

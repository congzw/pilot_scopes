using Microsoft.EntityFrameworkCore;
using SmartClass.Web.Chats.Domain;

namespace SmartClass.Web.Chats
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Room>(entity =>
            {
                entity.HasKey(x => x.RoomId);
            });

            builder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserId);
            });

            builder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(x => new { x.Group, x.UserId }).HasName("PK_UserGroup");
            });

        }
    }
}

using System;
using System.Linq;
using SmartClass.Web.Chats.Domain;

namespace SmartClass.Web.Chats.AppServices
{
    public interface ISeedAppService
    {
        void Seed(SeedArgs args);
    }

    public class SeedArgs
    {
        public int RoomCount { get; set; }
        public int UserCount { get; set; }
        public bool DeleteExist { get; set; }
    }


    public class SeedAppService : ISeedAppService
    {
        private readonly ChatDbContext _chatDbContext;

        public SeedAppService(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public void Seed(SeedArgs args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.DeleteExist)
            {
                _chatDbContext.Database.EnsureDeleted();
            }
            _chatDbContext.Database.EnsureCreated();

            var users = _chatDbContext.Users.ToList();
            if (users.Count == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    _chatDbContext.Users.Add(new User() { UserId = "User" + i, UserName = "用户" + i });
                }

                _chatDbContext.SaveChanges();
            }

            var rooms = _chatDbContext.Rooms.ToList();
            if (rooms.Count == 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    _chatDbContext.Rooms.Add(new Room() {RoomId = "Room" + i, RoomName = "房间" + i});
                }
                _chatDbContext.SaveChanges();
            }
        }
    }
}

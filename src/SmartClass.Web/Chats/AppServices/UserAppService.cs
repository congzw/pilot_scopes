using System.Collections.Generic;
using System.Linq;
using SmartClass.Web.Chats.Domain;

namespace SmartClass.Web.Chats.AppServices
{
    public interface IUserAppService
    {
        IList<User> GetUsers();
    }

    public class UserAppService : IUserAppService
    {
        private readonly ChatDbContext _chatDbContext;

        public UserAppService(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }


        public IList<User> GetUsers()
        {
            return _chatDbContext.Users.ToList();
        }
    }
}

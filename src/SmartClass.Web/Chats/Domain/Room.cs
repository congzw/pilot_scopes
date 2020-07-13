using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartClass.Web.Chats.Domain
{
    public class Room
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        //public IList<UserGroup> Groups { get; set; } = new List<UserGroup>();
    }
}
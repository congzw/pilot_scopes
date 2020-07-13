using System.Collections.Generic;
using System.Linq;
using SmartClass.Web.Chats.Domain;

namespace SmartClass.Web.Chats.AppServices
{
    public interface IRoomAppService
    {
        IList<RoomDto> GetRooms();
        void CreateRoom(Room room);
        void ResetRoom(ResetRoomArgs args);
        void DeleteRoom(DeleteRoomArgs args);
        void JoinRoom(JoinRoomArgs args);
        void LeaveRoom(LeaveRoomArgs args);
    }

    public class RoomDto
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
    }


    public class LeaveRoomArgs
    {
    }

    public class JoinRoomArgs
    {
    }

    public class DeleteRoomArgs
    {
        public string RoomId { get; set; }
    }

    public class ResetRoomArgs
    {
        public string RoomId { get; set; }
    }

    public class RoomAppService : IRoomAppService
    {
        private readonly ChatDbContext _chatDbContext;

        public RoomAppService(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public IList<RoomDto> GetRooms()
        {
            var rooms = _chatDbContext.Rooms
                .Select(x => new RoomDto
                {
                    RoomName = x.RoomName,
                    RoomId = x.RoomId
                }).ToList();
            return rooms;
        }

        public void CreateRoom(Room room)
        {
            throw new System.NotImplementedException();
        }

        public void ResetRoom(ResetRoomArgs args)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteRoom(DeleteRoomArgs args)
        {
            throw new System.NotImplementedException();
        }

        public void JoinRoom(JoinRoomArgs args)
        {
            throw new System.NotImplementedException();
        }

        public void LeaveRoom(LeaveRoomArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}
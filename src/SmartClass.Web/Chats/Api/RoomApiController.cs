using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SmartClass.Web.Chats.AppServices;
using SmartClass.Web.Chats.Domain;

namespace SmartClass.Web.Chats.Api
{
    [Route("api/chat/room")]
    [ApiController]
    public class RoomApiController
    {
        private readonly IRoomAppService _roomAppService;

        public RoomApiController(IRoomAppService roomAppService)
        {
            _roomAppService = roomAppService;
        }

        [Route("GetDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }

        [Route("GetRooms")]
        [HttpGet]
        public IList<RoomDto> GetRooms()
        {
            var rooms = _roomAppService.GetRooms();
            return rooms;
        }

        public void CreateRoom(Room room)
        {
            throw new NotImplementedException();
        }

        public void ResetRoom(ResetRoomArgs args)
        {
            throw new NotImplementedException();
        }

        public void DeleteRoom(DeleteRoomArgs args)
        {
            throw new NotImplementedException();
        }

        public void JoinRoom(JoinRoomArgs args)
        {
            throw new NotImplementedException();
        }

        public void LeaveRoom(LeaveRoomArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
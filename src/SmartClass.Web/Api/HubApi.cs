using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.Applications;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;

namespace SmartClass.Web.Api
{
    [Route("api/hub")]
    [ApiController]
    public class HubApiController : ControllerBase
    {
        private readonly SignalREventBus _bus;
        private readonly IHubContext<_AnyHub> _hubContext;
        private readonly IClientMonitor _clientMonitor;

        public HubApiController(SignalREventBus bus, IHubContext<_AnyHub> hubContext, IClientMonitor clientMonitor)
        {
            _bus = bus;
            _hubContext = hubContext;
            _clientMonitor = clientMonitor;
        }

        [Route("getDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }
        [Route("ClientStub")]
        [HttpPost]
        public async Task<string> ClientStub(SendContext sendContext)
        {
            var args = new ClientMethodArgs();
            args.SendContext = sendContext;
            args.Method = "updateMessage";
            args.MethodArgs = new { message = "From Server message" };

            await _bus.Raise(new ClientStubEvent(_hubContext.AsHubContextWrapper(), args));
            return "OK";
        }

        [Route("GetClientGroups")]
        [HttpGet]
        public Task<IList<ScopeClientGroup>> GetClientGroups([FromQuery]GetClientGroupsArgs args)
        {
            return _clientMonitor.GetClientGroups(args);
        }

        [Route("AddToGroup")]
        [HttpGet]
        public Task<bool> AddToGroup(string scopeId, string groupId, string clientId)
        {
            var joinGroupArgs = new JoinGroupArgs()
            {
                ScopeId = scopeId,
                Group = groupId
            };
            joinGroupArgs.ClientIds.Add(clientId);

            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            _clientMonitor.JoinGroup(new JoinGroupEvent(_hubContext.AsHubContextWrapper(), sendContext,  joinGroupArgs));
            return Task.FromResult(true);
        }

        [Route("LeaveGroup")]
        [HttpPost]
        public Task<bool> LeaveGroup(string scopeId, string groupId, string clientId)
        {
            var leaveGroupArgs = new LeaveGroupArgs()
            {
                ScopeId = scopeId,
                Group = groupId
            };
            leaveGroupArgs.ClientIds.Add(clientId);

            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            //_clientMonitor.LeaveGroup(new LeaveGroupEvent(_scopeHub, leaveGroupArgs));
            return Task.FromResult(true);
        }

        //[Route("ClientStub")]
        //[HttpGet]
        //public async Task<string> ClientStub(string scopeId, string groupId, string clientId, string message)
        //{
        //    if (string.IsNullOrWhiteSpace(scopeId))
        //    {
        //        return "BAD SCOPE!";
        //    }

        //    //todo: read SendFrom from token
        //    var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
        //    sendContext.To.ScopeId = scopeId;
        //    sendContext.From.ClientId = "default";
        //    //for demo!
        //    var args = new ClientMethodArgs();
        //    if (!string.IsNullOrEmpty(groupId))
        //    {
        //        sendContext.To.Groups.Add(groupId);
        //    }
        //    if (!string.IsNullOrEmpty(clientId))
        //    {
        //        sendContext.To.ClientIds.Add(clientId);
        //    }

        //    args.SendContext = sendContext;
        //    args.Method = "updateMessage";
        //    //args.SetBagValue("foo", "From Server foo");
        //    args.MethodArgs = new { message = "From Server message" };

        //    await _bus.Raise(new ClientStubEvent(_hubContext.AsHubContextWrapper(),  args));
        //    return "OK";
        //}

        //[Route("ClientStub")]
        //[HttpPost]
        //public async Task<string> ClientStub(SendContext sendContext)
        //{
        //    var args = new ClientMethodArgs();
        //    args.SendContext = sendContext;
        //    args.Method = "updateMessage";
        //    args.MethodArgs = new { message = "From Server message" };

        //    await _bus.Raise(new ClientStubEvent(_hubContext.AsHubContextWrapper(), args));
        //    return "OK";
        //}

        //[Route("AddToGroup")]
        //[HttpGet]
        //public Task<bool> AddToGroup(string scopeId, string groupId, string clientId)
        //{
        //    var joinGroupArgs = new JoinGroupArgs()
        //    {
        //        ScopeId = scopeId,
        //        Group = groupId
        //    };
        //    joinGroupArgs.ClientIds.Add(clientId);

        //    var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
        //    _clientMonitor.JoinGroup(new JoinGroupEvent(_hubContext.AsHubContextWrapper(), sendContext,  joinGroupArgs));
        //    return Task.FromResult(true);
        //}
    }
}

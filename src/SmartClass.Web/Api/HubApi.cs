using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.Applications;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;
using SmartClass.Common.Scopes;

namespace SmartClass.Web.Api
{
    [Route("api/hub")]
    [ApiController]
    public class HubApiController : ControllerBase
    {
        private readonly SignalREventBus _bus;
        private readonly IHubContext<_AnyHub> _hubContext;
        private readonly IScopeClientGroupRepository _scopeClientGroupRepository;

        public HubApiController(SignalREventBus bus, IHubContext<_AnyHub> hubContext, IScopeClientGroupRepository scopeClientGroupRepository)
        {
            _bus = bus;
            _hubContext = hubContext;
            _scopeClientGroupRepository = scopeClientGroupRepository;
        }

        [Route("getDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }

        #region 消息发送
        [Route("ClientMethod")]
        [HttpPost]
        public async Task<string> ClientMethod(SendContext sendContext)
        {
            var args = new ClientMethodArgs();
            args.SendContext = sendContext;
            args.Method = "updateMessage";                 //
            args.MethodArgs = new { message = "From Server message" };

            await _bus.Raise(new ClientMethodEvent(_hubContext.AsHubContextWrapper(), args));
            return "OK";
        }
        #endregion

        #region 组的操作

        [Route("GetClientGroups")]
        [HttpGet]
        public Task<IList<ScopeClientGroup>> GetClientGroups([FromQuery]GetClientGroupsArgs args)
        {
            var scopeClientGroups = _scopeClientGroupRepository.GetScopeClientGroups(args);
            return Task.FromResult(scopeClientGroups);
        }

        [Route("AddToGroup")]
        [HttpGet]
        public async Task AddToGroup(string scopeId, string groupId, string clientId)
        {
            var joinGroupArgs = new JoinGroupArgs()
            {
                ScopeId = scopeId,
                Group = groupId
            };
            joinGroupArgs.ClientIds.Add(clientId);

            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            await _bus.Raise(new JoinGroupEvent(_hubContext.AsHubContextWrapper(), sendContext, joinGroupArgs));
        }

        [Route("LeaveGroup")]
        [HttpGet]
        public async Task LeaveGroup(string scopeId, string groupId, string clientId)
        {
            var leaveGroupArgs = new LeaveGroupArgs()
            {
                ScopeId = scopeId,
                Group = groupId
            };
            leaveGroupArgs.ClientIds.Add(clientId);

            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            await _bus.Raise(new LeaveGroupEvent(_hubContext.AsHubContextWrapper(), sendContext, leaveGroupArgs));
        }

        #endregion

        #region scope 操作
        [Route("GetScopeContexts")]
        [HttpGet]
        public Task<IList<ScopeContext>> GetScopeContexts()
        {
            var scopeRepository = ScopeContext.Resolve();
            var scopeContexts = scopeRepository.GetScopeContexts();
            return Task.FromResult(scopeContexts);
        }

        [Route("GetScopeContext")]
        [HttpGet]
        public Task<ScopeContext> GetScopeContext(string scopeId)
        {
            var scopeContext = ScopeContext.GetScopeContext(scopeId,false);
            return Task.FromResult(scopeContext);
        }

        [Route("ResetScope")]
        [HttpGet]
        public async Task ResetScope(string scopeId)
        {
            var resetScopeArgs = new ResetScopeArgs()
            {
                ScopeId = scopeId
            };
            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            await _bus.Raise(new ResetScopeEvent(_hubContext.AsHubContextWrapper(), sendContext, resetScopeArgs));
        }

        [Route("UpdateScope")]
        [HttpGet]
        public async Task UpdateScope(string scopeId)
        {
            var updateScopeArgs = new UpdateScopeArgs()
            {
                ScopeId = scopeId
            };
            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            await _bus.Raise(new UpdateScopeEvent(_hubContext.AsHubContextWrapper(), sendContext, updateScopeArgs));
        }

        #endregion


    }
}

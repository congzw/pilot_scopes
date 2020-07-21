using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;
using SmartClass.Common.ScopeHubs._Impl;
using SmartClass.Common.Scopes;

namespace SmartClass.Web.Api
{
    [Route("api/hub")]
    [ApiController]
    public class HubApiController : ControllerBase
    {
        private readonly SignalREventBus _bus;
        private readonly IHubContextWrapperHold _hubContextWrapperHold;
        private readonly IScopeClientGroupRepository _scopeClientGroupRepository;

        public HubApiController(SignalREventBus bus, IHubContextWrapperHold hubContextWrapperHold, IScopeClientGroupRepository scopeClientGroupRepository)
        {
            _bus = bus;
            _hubContextWrapperHold = hubContextWrapperHold;
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

            await _bus.Raise(new ClientMethodEvent(_hubContextWrapperHold.Wrapper, args));
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
            await _bus.Raise(new JoinGroupEvent(_hubContextWrapperHold.Wrapper, sendContext, joinGroupArgs));
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
            await _bus.Raise(new LeaveGroupEvent(_hubContextWrapperHold.Wrapper, sendContext, leaveGroupArgs));
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
            var scopeContext = ScopeContext.GetScopeContext(scopeId, false);
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
            await _bus.Raise(new ResetScopeEvent(_hubContextWrapperHold.Wrapper, sendContext, resetScopeArgs));
        }

        [Route("UpdateScope")]
        [HttpGet]
        public async Task UpdateScope(string scopeId)
        {
            var updateScopeArgs = new UpdateScopeArgs()
            {
                ScopeId = scopeId,
            };
            updateScopeArgs.Bags["testKey"] = "testValue" + DateTime.Now.ToString("yyyy-mm-dd HH:MM:SS");
            var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            await _bus.Raise(new UpdateScopeEvent(_hubContextWrapperHold.Wrapper, sendContext, updateScopeArgs));
        }

        [Route("NotifyScope")]
        [HttpGet]
        public async Task NotifyScope(string scopeId)
        {
            var args = new ClientMethodArgs().ForNotify(new { message = "NotifyScope message" + DateTime.Now.ToString("yyyy-mm-dd HH:MM:SS") });
            var sendContext = new SendContext();
            sendContext.From.ScopeId = scopeId;
            args.SendContext = sendContext;
            await _bus.Raise(new ClientMethodEvent(_hubContextWrapperHold.Wrapper, args));
        }
        #endregion


    }
}

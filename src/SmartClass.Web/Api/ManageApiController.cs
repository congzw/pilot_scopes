using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.Applications;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Web.Api
{
    [Route("api/manage")]
    [ApiController]
    public class ManageApiController : ControllerBase
    {
        private readonly SignalREventBus _bus;
        private readonly IHubContext<_AnyHub> _hubContext;

        public ManageApiController(SignalREventBus bus, IHubContext<_AnyHub> hubContext)
        {
            _bus = bus;
            _hubContext = hubContext;
        }

        [Route("GetScopes")]
        [HttpGet]
        public IList<ScopeContext> GetScopes()
        {
            var scopeRepository = ScopeContext.Resolve();
            var scopeContexts = scopeRepository.GetScopeContexts();
            return scopeContexts;
        }

        [Route("ResetScopes")]
        [HttpGet]
        public async Task<string> ResetScopes(string scopeId)
        {
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                return "BAD SCOPE";
            }
            
            var scopeContext = ScopeContext.GetScopeContext(scopeId, false);
            if (scopeContext == null)
            {
                return "Not Find Scope " + scopeId;
            }

            var sendContext = SendContext.Create()
                .WithSendFrom(new SendFrom().WithScopeId(scopeId));
            var args = new ResetScopeArgs().WithScopeId(scopeId);
            await _bus.Raise(new ResetScopeEvent(_hubContext.AsHubContextWrapper(), sendContext, args));
            return "OK";
        }

        [Route("ClientMethod")]
        [HttpGet]
        public async Task<string> ClientMethod(string scopeId, string clientIds)
        {
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                return "BAD SCOPE";
            }

            //var sendContext = new SendFrom().WithScopeId(scopeId).GetSendContext();
            var sendContext = SendContext.Create()
                .WithSendFrom(new SendFrom().WithScopeId(scopeId));

            if (!string.IsNullOrWhiteSpace(clientIds))
            {
                sendContext.To.ClientIds = clientIds.Split(',').Distinct().ToList();
            }
            else
            {
                sendContext.WithSendTo(SendTo.CreateForScopeGroupAll(scopeId));
            }

            var args = ClientMethodArgs.Create().ForLogMessage(new { message = "From Server ClientMethod" });
            args.SendContext = sendContext;

            await _bus.Raise(new ClientMethodEvent(_hubContext.AsHubContextWrapper(),  args));
            return "OK";
        }

    }
}
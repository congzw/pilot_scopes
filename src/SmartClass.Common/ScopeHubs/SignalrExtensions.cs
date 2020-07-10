using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs
{
    public static class SignalrExtensions
    {
        public static HttpContext TryGetHttpContext(this Hub hub)
        {
            return hub?.Context?.GetHttpContext();
        }

        private static string TryGetScopeId(this Hub hub)
        {
            //todo: read from token
            return hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.ScopeId_Default);
        }

        private static string TryGetClientId(this Hub hub)
        {
            //todo: read from token
            ////post args : {"scopeId" : "abc"}
            //var user = hub.Context.User;
            //var identityName = user.Identity.Name;
            //var scopeId = user.Claims;
            var clientId = hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ClientId, (string)null);
            return clientId;
        }

        private static string TryGetClientType(this Hub hub)
        {
            //todo: read from token
            return string.Empty;
        }

        public static SignalREventCallingContext GetSignalREventContext(this HttpContext httpContext)
        {
            var scopeId = httpContext.TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.ScopeId_Default);
            var clientId = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientId, string.Empty);
            var userId = string.Empty;
            var clientType = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientType, string.Empty);

            ////todo: read from claim
            //var user = httpContext.User;
            //var scopeId = user.FindFirst("ScopeId").Value;
            //var clientId = user.FindFirst("ClientId").Value;
            //var clientType = user.FindFirst("ClientType").Value;
            //var userId = user.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

            var ctx = new SignalREventCallingContext();
            ctx.ScopeId = scopeId;
            ctx.ClientId = clientId;
            ctx.UserId = userId;
            ctx.ClientType = clientType;

            return ctx;
        }

        public static SignalREventCallingContext GetSignalREventContext(this Hub hub)
        {
            return hub.TryGetHttpContext().GetSignalREventContext();
        }
        
        public static THub FixScopeIdForArgs<THub>(this THub hub, IScopeKey args) where THub : Hub
        {
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                args.ScopeId = hub.TryGetScopeId();
            }
            return hub;
        }
    }
}

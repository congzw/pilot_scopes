using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public static string TryGetScopeId(this Hub hub)
        {
            //todo: read from token
            return hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.ScopeId_Default);
        }

        public static string TryGetClientId(this Hub hub)
        {
            //todo: read from token
            ////post args : {"scopeId" : "abc"}
            //var user = hub.Context.User;
            //var identityName = user.Identity.Name;
            //var scopeId = user.Claims;
            var clientId = hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ClientId, (string)null);
            return clientId;
        }

        public static string TryGetClientType(this Hub hub)
        {
            //todo: read from token
            return string.Empty;
        }

        public static SignalREventCallingContext GetSignalREventContext(this HttpContext httpContext)
        {
            var scopeId = httpContext.TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.ScopeId_Default);
            var clientId = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientId, string.Empty);
            var userId = string.Empty;

            ////todo: read from claim
            //var user = httpContext.User;
            //var scopeId = user.FindFirst("ScopeId").Value;
            //var clientId = user.FindFirst("ClientId").Value;
            //var userId = user.Claims.Single(x => x.Type == ClaimTypes.Name).Value;

            var ctx = new SignalREventCallingContext();
            ctx.ScopeId = scopeId;
            ctx.ClientId = clientId;
            ctx.UserId = userId;

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

    public class SignalREventCallingContext : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
    }
}

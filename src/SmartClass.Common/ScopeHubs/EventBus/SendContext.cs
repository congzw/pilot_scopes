using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable once CheckNamespace
namespace SmartClass.Common.ScopeHubs
{
    public class SendContext
    {
        public SendFrom From { get; set; } = new SendFrom();
        public SendTo To { get; set; } = new SendTo();
    }

    public class SendFrom : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string ClientType { get; set; }
    }

    public class SendTo : IScopeKey
    {
        public string ScopeId { get; set; }
        public IList<string> ClientIds { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();
        public static SendTo CreateForScopeGroupAll(string scopeId)
        {
            var args = new SendTo();
            args.WithScopeId(scopeId);
            args.Groups.Add(HubConst.GroupName_All);
            return args;
        }
    }

    public static class SendContextExtensions
    {
        public static SendFrom GetSendFrom(this HttpContext httpContext)
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

            var ctx = new SendFrom();
            ctx.ScopeId = scopeId;
            ctx.ClientId = clientId;
            ctx.UserId = userId;
            ctx.ClientType = clientType;

            return ctx;
        }

        public static SendFrom GetSendFrom(this Hub hub)
        {
            return hub.TryGetHttpContext().GetSendFrom();
        }

        public static SendContext GetSendContext(this SendFrom sendFrom)
        {
            return new SendContext { From = sendFrom };
        }
    }
}
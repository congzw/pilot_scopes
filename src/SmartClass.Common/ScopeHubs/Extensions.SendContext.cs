using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs
{
    public static class SendContextExtensions
    {
        public static THub FixScopeIdForArgs<THub>(this THub hub, IScopeKey args) where THub : Hub
        {
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                args.ScopeId = hub.GetSendFrom().ScopeId;
            }
            return hub;
        }

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

        public static List<string> GetFullNameGroups(this SendTo sendTo)
        {
            if (sendTo == null) throw new ArgumentNullException(nameof(sendTo));
            if (sendTo.Groups.IsNullOrEmpty())
            {
                return new List<string>();
            }
            return sendTo.Groups.Select(x => ScopeGroupName.GetScopedGroup(sendTo.ScopeId, x).ToScopeGroupFullName()).Distinct().ToList();
        }

        public static SendContext GetSendContext(this SendFrom sendFrom)
        {
            return new SendContext { From = sendFrom };
        }

        public static SendContext WithSendFrom(this SendContext sendContext, SendFrom sendFrom)
        {
            sendContext.From = sendFrom;
            return sendContext;
        }
        public static SendContext WithSendTo(this SendContext sendContext, SendTo sendTo)
        {
            sendContext.To = sendTo;
            return sendContext;
        }
        public static SendContext AutoFixToGroupAllIfEmpty(this SendContext sendContext)
        {
            if (sendContext.From == null)
            {
                return sendContext;
            }

            if (sendContext.To == null)
            {
                sendContext.To = new SendTo();
            }

            if (string.IsNullOrWhiteSpace(sendContext.To.ScopeId))
            {
                sendContext.To.ScopeId = sendContext.From.ScopeId;
            }

            if (!string.IsNullOrWhiteSpace(sendContext.To.ScopeId))
            {
                if (sendContext.To.IsEmptyClientsAndGroups())
                {
                    sendContext.To.Groups.Add(HubConst.GroupName_All);
                }
            }

            return sendContext;
        }
    }
}

using System;
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



        public static string GetCurrentScope<THub>(this THub hub) where THub : Hub
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }

            //query >= items >= default
            var scope = hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, (string)null);
            if (scope != null)
            {
                hub.Context.Items[HubConst.Args_ScopeId] = scope;
                return scope;
            }

            if (hub.Context.Items.ContainsKey(HubConst.Args_ScopeId))
            {
                return hub.Context.Items[HubConst.Args_ScopeId] as string;
            }

            hub.Context.Items[HubConst.Args_ScopeId] = HubConst.ScopeId_Default;
            return hub.Context.Items[HubConst.Args_ScopeId] as string;
        }

        public static THub FixScopeIdForContext<THub>(this THub hub, string scopeId = null) where THub : Hub
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }

            if (string.IsNullOrWhiteSpace(scopeId))
            {
                scopeId = hub.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.ScopeId_Default);
            }
            hub.Context.Items[HubConst.Args_ScopeId] = scopeId;
            return hub;
        }

        public static THub FixScopeIdForArgs<THub>(this THub hub, IScopeKey args) where THub : Hub
        {
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                args.ScopeId = hub.GetCurrentScope();
            }
            return hub;
        }
    }
}

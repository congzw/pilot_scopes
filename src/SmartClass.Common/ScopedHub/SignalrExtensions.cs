using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.Http;

namespace SmartClass.Common.ScopedHub
{
    public static class SignalRExtensions
    {
        public static HttpContext TryGetHttpContext(this Hub hub)
        {
            return hub?.Context?.GetHttpContext();
        }

        public static string TryGetScopeId(this Hub hub)
        {
            return hub?.Context?.GetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.DefaultScopeId);
        }
    }
}

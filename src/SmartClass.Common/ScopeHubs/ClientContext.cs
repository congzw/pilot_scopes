using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SmartClass.Common.ScopeHubs
{
    public class ClientContext
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string ClientType { get; set; }
        
        public static ClientContext GetCurrentClientContext(HttpContext httpContext)
        {
            return Resolve().GetCurrent(httpContext);
        }

        #region for extensions

        private static readonly Lazy<IClientContextService> _lazy = new Lazy<IClientContextService>(() => new RequestClientContextService());
        public static Func<IClientContextService> Resolve = () => _lazy.Value;

        #endregion
    }

    #region for extensions

    public interface IClientContextService
    {
        ClientContext GetCurrent(HttpContext httpContext);
    }

    public class RequestClientContextService : IClientContextService
    {
        public ClientContext GetCurrent(HttpContext httpContext)
        {
            var scopeId = httpContext.TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.ScopeId_Default);
            var clientId = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientId, string.Empty);
            var userId = string.Empty;
            var clientType = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientType, string.Empty);
            
            var ctx = new ClientContext();
            ctx.ScopeId = scopeId;
            ctx.ClientId = clientId;
            ctx.UserId = userId;
            ctx.ClientType = clientType;

            return ctx;
        }
    }

    public class TokenClientContextService : IClientContextService
    {
        public ClientContext GetCurrent(HttpContext httpContext)
        {
            var user = httpContext.User;
            var scopeId = user.FindFirst("ScopeId").Value;
            var clientId = user.FindFirst("ClientId").Value;
            var clientType = user.FindFirst("ClientType").Value;
            var userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            var ctx = new ClientContext();
            ctx.ScopeId = scopeId;
            ctx.ClientId = clientId;
            ctx.UserId = userId;
            ctx.ClientType = clientType;

            return ctx;
        }
    }

    #endregion

}

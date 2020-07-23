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

    public interface IClientContextService : IMyScoped
    {
        ClientContext GetCurrent(HttpContext httpContext);
    }
    
    public class RequestClientContextService : IClientContextService
    {
        public ClientContext GetCurrent(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                var httpContextAccessor = ServiceLocator.Current.GetService<IHttpContextAccessor>();
                httpContext = httpContextAccessor.HttpContext;
            }

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var user = httpContext.User;

            var scopeId = httpContext.TryGetQueryParameterValue(HubConst.Args_ScopeId, string.Empty);
            if (string.IsNullOrWhiteSpace(scopeId))
            {
                scopeId = user.FindFirstValue("ScopeId");
            }
            var clientId = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientId, string.Empty);
            if (string.IsNullOrWhiteSpace(clientId))
            {
                clientId = user.FindFirstValue("ClientId");
            }
            var userId = httpContext.TryGetQueryParameterValue(HubConst.Args_UserId, string.Empty);
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            }

            var clientType = httpContext.TryGetQueryParameterValue(HubConst.Args_ClientType, string.Empty);
            if (string.IsNullOrWhiteSpace(clientType))
            {
                clientType = user.FindFirstValue("ClientType");
            }
            
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

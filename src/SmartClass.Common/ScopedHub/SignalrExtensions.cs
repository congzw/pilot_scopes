using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopedHub
{
    public static class SignalRExtensions
    {
        public static HttpContext TryGetHttpContext(this Hub hub)
        {
            return hub?.Context?.GetHttpContext();
        }
    }
}

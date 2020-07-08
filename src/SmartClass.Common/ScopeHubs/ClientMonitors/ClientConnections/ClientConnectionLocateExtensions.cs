using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public static class ClientConnectionLocateExtensions
    {
        public static ClientConnectionLocate TryGetClientConnectionLocate(this Hub hub)
        {
            var locate = new ClientConnectionLocate();
            locate.ScopeId = hub.TryGetScopeId();
            locate.ClientId = hub.TryGetClientId();
            locate.ConnectionId = hub.Context.ConnectionId;
            return locate;
        }
        
        public static T WithConnectionId<T>(this T instance, string connectionId) where T : IClientConnectionLocate
        {
            if (instance == null)
            {
                return default(T);
            }
            instance.ConnectionId = connectionId;
            return instance;
        }

        public static string ToLocateDesc(this IClientConnectionLocate locate)
        {
            return string.Format("{0},{1},{2}", locate.ScopeId, locate.ClientId, locate.ConnectionId);
        }

        public static T Locate<T>(this IEnumerable<T> locates, IClientConnectionLocate locate) where T : IClientConnectionLocate
        {
            if (locates == null || locate == null)
            {
                return default(T);
            }
            
            // ReSharper disable once PossibleMultipleEnumeration
            var theOne =  locates.FirstOrDefault(x => locate.ScopeId.MyEquals(x.ScopeId) && locate.ClientId.MyEquals(x.ClientId));
            if (theOne == null)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                theOne =  locates.SingleOrDefault(x => locate.ConnectionId.MyEquals(x.ConnectionId));
            }
            return theOne;
        }
    }
}

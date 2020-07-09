using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopeHubs
{
    public static class LocateKeysExtensions
    {
        public static T WithScopeId<T>(this T theObject, string scopeId) where T : IScopeKey
        {
            if (theObject == null)
            {
                return default(T);
            }
            theObject.ScopeId = scopeId;
            return theObject;
        }
        public static T WithClientId<T>(this T theObject, string clientId) where T : IClientKey
        {
            if (theObject == null)
            {
                return default(T);
            }
            theObject.ClientId = clientId;
            return theObject;
        }
        public static T WithGroup<T>(this T theObject, string group) where T : IGroupKey
        {
            if (theObject == null)
            {
                return default(T);
            }
            theObject.Group = group;
            return theObject;
        }
        public static T WithConnectionId<T>(this T theObject, string connId) where T : ISignalRConnectionKey
        {
            if (theObject == null)
            {
                return default(T);
            }
            theObject.ConnectionId = connId;
            return theObject;
        }
    }
}

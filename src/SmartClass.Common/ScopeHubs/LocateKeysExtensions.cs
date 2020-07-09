using System;
using System.Collections.Generic;
using System.Linq;

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

        public static T CopyFrom<T>(this T updateObject, IScopeClientLocate fromObject) where T : IScopeClientLocate
        {
            if (updateObject == null) throw new ArgumentNullException(nameof(updateObject));
            if (fromObject == null) throw new ArgumentNullException(nameof(fromObject));

            updateObject.WithScopeId(fromObject.ScopeId);
            updateObject.WithClientId(fromObject.ClientId);
            return updateObject;
        }
        public static T CopyFrom<T>(this T updateObject, IScopeGroupLocate fromObject) where T : IScopeGroupLocate
        {
            if (updateObject == null) throw new ArgumentNullException(nameof(updateObject));
            if (fromObject == null) throw new ArgumentNullException(nameof(fromObject));

            updateObject.WithScopeId(fromObject.ScopeId);
            updateObject.WithGroup(fromObject.Group);
            return updateObject;
        }

        public static string ToScopeGroupFullName(this IScopeGroupLocate locate)
        {
            return string.Format("{0}.{1}", locate.ScopeId, locate.Group);
        }

        public static bool SameLocateKey(this IScopeClientLocate locate, IScopeClientLocate locate2)
        {
            if (locate == null || locate2 == null)
            {
                return false;
            }
            return locate.ScopeId.MyEquals(locate2.ScopeId) && locate.ClientId.MyEquals(locate2.ClientId);
        }
        public static bool SameLocateKey(this IScopeGroupLocate locate, IScopeGroupLocate locate2)
        {
            if (locate == null || locate2 == null)
            {
                return false;
            }
            return locate.ScopeId.MyEquals(locate2.ScopeId) && locate.Group.MyEquals(locate2.Group);
        }

        public static T Locate<T>(this IEnumerable<T> locates, IScopeClientLocate locateArgs) where T : IScopeClientLocate
        {
            return locates.FirstOrDefault(x => x.SameLocateKey(locateArgs));
        }
        public static T Locate<T>(this IEnumerable<T> locates, IScopeGroupLocate locateArgs) where T : IScopeGroupLocate
        {
            return locates.FirstOrDefault(x => x.SameLocateKey(locateArgs));
        }
    }
}

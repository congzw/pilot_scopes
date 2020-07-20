using System;

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
        
        public static string CreateCompareLocateValue<T>(this T locate) where T : IScopeKey
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));
            //IScopeKey, IClientKey, IGroupKey, ISignalRConnectionKey
            string desc = locate.ScopeId;

            if (locate is IGroupKey theGroup)
            {
                desc += ",";
                desc += theGroup.Group;
            }

            if (locate is IClientKey theClient)
            {
                desc += ",";
                desc += theClient.ClientId;
            }

            if (locate is ISignalRConnectionKey theConn)
            {
                desc += ",";
                desc += theConn.ConnectionId;
            }
            return desc;
        }
        public static bool SameLocateValue<T>(this T locate, T locate2) where T : IScopeKey
        {
            return locate.CreateCompareLocateValue().MyEquals(locate2.CreateCompareLocateValue());
        }
        public static bool AllLocateKeysHasValue<T>(this T locate, out string msg) where T : IScopeKey
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));

            //IScopeKey, IClientKey, IGroupKey, ISignalRConnectionKey      
            msg = "";
            if (locate is IScopeKey theScope && string.IsNullOrWhiteSpace(theScope.ScopeId))
            {
                msg = string.Format("{0} has not no value", nameof(theScope.ScopeId));
                return false;
            }

            if (locate is IGroupKey theGroup && string.IsNullOrWhiteSpace(theGroup.Group))
            {
                msg = string.Format("{0} has not no value", nameof(theGroup.Group));
                return false;
            }

            if (locate is IClientKey theClient && string.IsNullOrWhiteSpace(theClient.ClientId))
            {
                msg = string.Format("{0} has not no value", nameof(theClient.ClientId));
                return false;
            }

            if (locate is ISignalRConnectionKey theConn && string.IsNullOrWhiteSpace(theConn.ConnectionId))
            {
                msg = string.Format("{0} has not no value", nameof(theConn.ConnectionId));
                return false;
            }

            return true;
        }
        public static string GetScopeClientKey(this IScopeClientLocate locate)
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));
            return string.Format("{0},{1}", locate.ScopeId, locate.ClientId);
        }

        public static string ToScopeGroupFullName(this IScopeGroupLocate locate)
        {
            return string.Format("{0}.{1}", locate.ScopeId, locate.Group);
        }
    }
}

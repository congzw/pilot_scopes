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

        //public static T CopyFrom<T>(this T updateObject, IScopeClientLocate fromObject) where T : IScopeClientLocate
        //{
        //    if (updateObject == null) throw new ArgumentNullException(nameof(updateObject));
        //    if (fromObject == null) throw new ArgumentNullException(nameof(fromObject));

        //    updateObject.WithScopeId(fromObject.ScopeId);
        //    updateObject.WithClientId(fromObject.ClientId);
        //    return updateObject;
        //}
        //public static T CopyFrom<T>(this T updateObject, IScopeGroupLocate fromObject) where T : IScopeGroupLocate
        //{
        //    if (updateObject == null) throw new ArgumentNullException(nameof(updateObject));
        //    if (fromObject == null) throw new ArgumentNullException(nameof(fromObject));

        //    updateObject.WithScopeId(fromObject.ScopeId);
        //    updateObject.WithGroup(fromObject.Group);
        //    return updateObject;
        //}
        //public static T CopyFrom<T>(this T updateObject, IScopeClientGroupLocate fromObject) where T : IScopeClientGroupLocate
        //{
        //    if (updateObject == null) throw new ArgumentNullException(nameof(updateObject));
        //    if (fromObject == null) throw new ArgumentNullException(nameof(fromObject));

        //    updateObject.WithScopeId(fromObject.ScopeId);
        //    updateObject.WithClientId(fromObject.ClientId);
        //    updateObject.WithGroup(fromObject.Group);
        //    return updateObject;
        //}
        //public static T CopyFrom<T>(this T updateObject, IClientConnectionLocate fromObject) where T : IClientConnectionLocate
        //{
        //    if (updateObject == null) throw new ArgumentNullException(nameof(updateObject));
        //    if (fromObject == null) throw new ArgumentNullException(nameof(fromObject));

        //    updateObject.WithScopeId(fromObject.ScopeId);
        //    updateObject.WithClientId(fromObject.ClientId);
        //    updateObject.WithConnectionId(fromObject.ConnectionId);
        //    return updateObject;
        //}

        public static string ToScopeGroupFullName(this IScopeGroupLocate locate)
        {
            return string.Format("{0}.{1}", locate.ScopeId, locate.Group);
        }
        public static IReadOnlyList<string> ToScopeGroupFullNames(this IEnumerable<IScopeGroupLocate> locates)
        {
            return locates.Select(x => x.ToScopeGroupFullName()).ToList();
        }


        public static string GetScopeClientKey(this IScopeClientLocate locate)
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));
            return string.Format("{0}.{1}", locate.ScopeId, locate.ClientId);
        }
        public static string GetScopeGroupKey(this IScopeGroupLocate locate)
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));
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
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }
        public static T Locate<T>(this IEnumerable<T> locates, IScopeClientLocate locateArgs) where T : IScopeClientLocate
        {
            return locates.FirstOrDefault(x => x.SameLocateKey(locateArgs));
        }
        public static T Locate<T>(this IEnumerable<T> locates, IScopeGroupLocate locateArgs) where T : IScopeGroupLocate
        {
            return locates.FirstOrDefault(x => x.SameLocateKey(locateArgs));
        }
        public static IEnumerable<T> LocateList<T>(this IList<T> locates, IList<IScopeClientLocate> locateArgs) where T : IScopeClientLocate
        {
            foreach (var locate in locates)
            {
                foreach (var locateArg in locateArgs)
                {
                    if (locateArg.SameLocateKey(locate))
                    {
                        yield return locate;
                    }
                }
            }
        }
        public static IEnumerable<T> LocateList<T>(this IList<T> locates, IList<IScopeGroupLocate> locateArgs) where T : IScopeGroupLocate
        {
            foreach (var locate in locates)
            {
                foreach (var locateArg in locateArgs)
                {
                    if (locateArg.SameLocateKey(locate))
                    {
                        yield return locate;
                    }
                }
            }
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
    }
}

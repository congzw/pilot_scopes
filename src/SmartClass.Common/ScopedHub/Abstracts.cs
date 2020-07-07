using System;

namespace SmartClass.Common.ScopedHub
{
    public interface IScopeKey
    {
        /// <summary>
        /// 连接端业务范围的唯一标识，例如某堂课的Id等场景。逻辑上划分的Hub范围
        /// </summary>
        string ScopeId { get; set; }
    }

    public interface IClientKey
    {
        /// <summary>
        /// 连接端Client的唯一标识，例如某设备的Id
        /// </summary>
        string ClientId { get; set; }
    }

    public interface IGroupKey
    {
        /// <summary>
        /// 组名
        /// </summary>
        string Group { get; set; }
    }

    public interface IConnectionKey
    {
        string ConnectionId { get; set; }
    }

    #region locates
    
    //scope client
    public interface IScopeClientLocate : IScopeKey, IClientKey
    {
    }
    //scope group
    public interface IScopeGroupLocate : IScopeKey, IGroupKey
    {
    }
    //scope client group
    public interface IScopeClientGroupLocate : IScopeKey, IClientKey, IGroupKey, IScopeClientLocate, IScopeGroupLocate
    {
    }
    //scope client connection
    public interface IScopeClientConnectionLocate : IScopeClientLocate, IConnectionKey
    {
        //scopeId + clientId => connId
    }
    
    public class ScopeClientLocate : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }

        public static ScopeClientLocate Create(string scopeId, string clientId)
        {
            if (scopeId == null) throw new ArgumentNullException(nameof(scopeId));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            return new ScopeClientLocate { ScopeId = scopeId, ClientId = clientId };
        }
    }
    public class ScopeGroupLocate : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }

        public static ScopeGroupLocate Create(string scopeId, string @group)
        {
            if (scopeId == null) throw new ArgumentNullException(nameof(scopeId));
            if (@group == null) throw new ArgumentNullException(nameof(@group));
            var theScope = string.IsNullOrWhiteSpace(scopeId) ? HubConst.DefaultScopeId : scopeId;
            var theGroup = string.IsNullOrWhiteSpace(group) ? HubConst.AllGroupId : group;
            return new ScopeGroupLocate { ScopeId = theScope, Group = theGroup };
        }
        public static ScopeGroupLocate CreateGroupAll(string scopeId)
        {
            return Create(scopeId, HubConst.AllGroupId);
        }
    }
    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }

        public static ScopeClientGroupLocate Create(string scopeId, string @group, string clientId)
        {
            if (scopeId == null) throw new ArgumentNullException(nameof(scopeId));
            if (@group == null) throw new ArgumentNullException(nameof(@group));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            return new ScopeClientGroupLocate { ScopeId = scopeId, Group = @group, ClientId = clientId };
        }
    }
    public class ScopeClientConnectionLocate : IScopeClientConnectionLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public static ScopeClientConnectionLocate Create(string scopeId, string clientId, string connId)
        {
            if (scopeId == null) throw new ArgumentNullException(nameof(scopeId));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            return new ScopeClientConnectionLocate { ScopeId = scopeId, ClientId = clientId, ConnectionId = connId };
        }
        public static ScopeClientConnectionLocate CreateWithConn(string connId)
        {
            return new ScopeClientConnectionLocate
            {
                ConnectionId = connId
            };
        }
    }

    #endregion
}

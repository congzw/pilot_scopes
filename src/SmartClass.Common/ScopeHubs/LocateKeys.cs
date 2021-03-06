﻿using System;
using SmartClass.Common.Scopes;

namespace SmartClass.Common.ScopeHubs
{
    public interface IScopeKey : IHaveScopeId
    {
    }

    public interface IClientKey
    {
        /// <summary>
        /// 连接端Client的唯一标识，例如某设备的Id
        /// </summary>
        string ClientId { get; set; }
    }

    public interface ISignalRConnectionKey
    {
        string ConnectionId { get; set; }
    }

    public interface IGroupKey
    {
        /// <summary>
        /// 组名
        /// </summary>
        string Group { get; set; }
    }

    //scope client
    public interface IScopeClientLocate : IScopeKey, IClientKey
    {
    }

    //scope client connection
    public interface IClientConnectionLocate : IScopeKey, IClientKey, ISignalRConnectionKey, IScopeClientLocate
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

    #region locate model

    public class ClientConnectionLocate : IClientConnectionLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }

        public static ClientConnectionLocate Create()
        {
            return new ClientConnectionLocate();
        }
        
        public static ClientConnectionLocate CreateFrom(IClientConnectionLocate locate)
        {
            if (locate == null) throw new ArgumentNullException(nameof(locate));

            var newLocate = new ClientConnectionLocate();
            newLocate.WithScopeId(locate.ScopeId);
            newLocate.WithClientId(locate.ClientId);
            newLocate.WithConnectionId(locate.ConnectionId);
            return newLocate;
        }
    }
    public class ScopeClientLocate : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public static ScopeClientLocate Create(string scopeId, string clientId)
        {
            return new ScopeClientLocate { ScopeId = scopeId, ClientId = clientId };
        }
    }
    public class ScopeGroupLocate : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }

        public static ScopeGroupLocate Create(string scopeId, string group)
        {
            return new ScopeGroupLocate { ScopeId = scopeId, Group = group };
        }
        public static ScopeGroupLocate CreateScopeGroupAll(string scopeId)
        {
            return Create(scopeId, HubConst.GroupName_All);
        }
    }
    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }

    #endregion
    
    public class ScopeGroupName
    {
        public static IScopeGroupLocate GetScopedGroup(string scopeId, string group)
        {
            var scopeGroupName = new ScopeGroupLocate();
            scopeGroupName.ScopeId = scopeId;
            scopeGroupName.Group = group;
            return scopeGroupName;
        }

        public static IScopeGroupLocate GetScopedGroupAll(string scopeId)
        {
            return GetScopedGroup(scopeId, HubConst.GroupName_All);
        }
    }
}

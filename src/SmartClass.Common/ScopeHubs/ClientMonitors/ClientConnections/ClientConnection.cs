﻿using System;
using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections
{
    public interface IClientConnection : IClientConnectionLocate
    {
        string ClientType { get; set; }
    }

    public class MyConnection : IClientConnection, IHaveBags
    {
        public MyConnection()
        {
            var now = GetDateNow();
            CreateAt = now;
            LastUpdateAt = now;
            ClientType = string.Empty;
            Bags = BagsHelper.Create();
            //Groups = new List<string>();
        }
        public Guid Id { get; set; }
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ClientType { get; set; }
        public string ConnectionId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdateAt { get; set; }

        public IDictionary<string, object> Bags { get; set; }

        public void CopyTo(MyConnection conn)
        {
            MyModelHelper.TryCopyProperties(conn, this);
            conn.Bags = this.Bags;
            //conn.Groups = this.Groups;
        }

        #region groups
        
        //public IList<string> Groups { get; set; }
        //public void AddScopeGroupIfNotExist(IScopeGroupLocate scopeGroupName)
        //{
        //    if (scopeGroupName == null) throw new ArgumentNullException(nameof(scopeGroupName));
        //    var groupName = scopeGroupName.ToScopeGroupFullName();
        //    var theOne = Groups.MyFind(groupName);
        //    if (string.IsNullOrWhiteSpace(theOne))
        //    {
        //        Groups.Add(groupName);
        //    }
        //}
        //public void RemoveScopeGroupIfExist(IScopeGroupLocate scopeGroupName)
        //{
        //    if (scopeGroupName == null) throw new ArgumentNullException(nameof(scopeGroupName));
        //    var groupName = scopeGroupName.ToScopeGroupFullName();
        //    var theOne = Groups.MyFind(groupName);
        //    if (!string.IsNullOrWhiteSpace(theOne))
        //    {
        //        Groups.Remove(theOne);
        //    }
        //}

        #endregion

        #region for extensions

        public static Func<DateTime> GetDateNow = () => DateHelper.Instance.GetDateNow();

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.Groups;

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
            Groups = new List<string>();
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
            conn.Groups = this.Groups;
        }

        #region groups
        
        public IList<string> Groups { get; set; }
        public void AddScopeGroupIfNotExist(ScopeGroupName scopeGroupName)
        {
            if (scopeGroupName == null) throw new ArgumentNullException(nameof(scopeGroupName));
            var groupName = scopeGroupName.ToFullName();
            var theOne = Groups.MyFind(groupName);
            if (string.IsNullOrWhiteSpace(theOne))
            {
                Groups.Add(groupName);
            }
        }
        public void RemoveScopeGroupIfExist(ScopeGroupName scopeGroupName)
        {
            if (scopeGroupName == null) throw new ArgumentNullException(nameof(scopeGroupName));
            var groupName = scopeGroupName.ToFullName();
            var theOne = Groups.MyFind(groupName);
            if (!string.IsNullOrWhiteSpace(theOne))
            {
                Groups.Remove(theOne);
            }
        }
        public async Task UpdateConnectionGroups(Hub hub)
        {
            var connectionId = this.ConnectionId;
            foreach (var group in this.Groups)
            {
                //已经处理过的group, 直接添加
                await hub.Groups.AddToGroupAsync(connectionId, group);
            }
        }

        #endregion

        #region for extensions

        public static Func<DateTime> GetDateNow = () => DateHelper.Instance.GetDateNow();

        #endregion
    }
}

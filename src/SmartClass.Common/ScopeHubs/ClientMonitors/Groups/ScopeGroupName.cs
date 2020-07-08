using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Groups
{
    public class ScopeGroupName : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }
        public string ToFullName()
        {
            return string.Format("{0}.{1}", ScopeId, Group);
        }
        public override string ToString()
        {
            return ToFullName();
        }

        public static ScopeGroupName GetScopedGroup(string scopeId, string group)
        {
            var scopeGroupName = new ScopeGroupName();
            scopeGroupName.ScopeId = scopeId;
            scopeGroupName.Group = group;
            return scopeGroupName;
        }
        public static ScopeGroupName GetScopedGroupAll(string scopeId)
        {
            return GetScopedGroup(scopeId, HubConst.GroupName_All);
        }
    }

    public static class ScopeGroupExtensions
    {
        public static Task AddToGroupAsync(this ScopeGroupName scopeGroupName, Hub hub)
        {
            var groupName = scopeGroupName.ToFullName();
            var connectionId = hub.Context.ConnectionId;
            return hub.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public static Task RemoveFromGroupAsync(this ScopeGroupName scopeGroupName, Hub hub)
        {
            var groupName = scopeGroupName.ToFullName();
            var connectionId = hub.Context.ConnectionId;
            return hub.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }


        //public static Task AddToGroupInScope(this IGroupManager groupManager, string connId, string group, string scopeId)
        //{
        //    var groupName = ScopeGroupHelper.Instance.GetScopedGroup(scopeId, group);
        //    return groupManager.AddToGroupAsync(connId, groupName);
        //}

        //public static Task RemoveFromGroupInScope(this IGroupManager groupManager, string connId, string group, string scopeId)
        //{
        //    var groupName = ScopeGroupHelper.Instance.GetScopedGroup(scopeId, group);
        //    return groupManager.RemoveFromGroupAsync(connId, groupName);
        //}
    }
}

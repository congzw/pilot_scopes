using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups
{
    internal class ScopeGroupFix
    {
        public static async Task OnConnected(Hub hub, string scopeId)
        {
            //Scope划分用
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(scopeId).ToScopeGroupFullName();
            await hub.Groups.AddToGroupAsync(hub.Context.ConnectionId, scopeGroup);

            //监控连接用
            var monitorScopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            await hub.Groups.AddToGroupAsync(hub.Context.ConnectionId, monitorScopeGroup);
        }

        public static async Task OnDisconnected(Hub hub, string scopeId)
        {
            //Scope划分用
            var scopeGroup = ScopeGroupName.GetScopedGroupAll(scopeId).ToScopeGroupFullName();
            await hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, scopeGroup);

            //监控连接用
            var monitorScopeGroup = ScopeGroupName.GetScopedGroupAll(HubConst.Monitor_ScopeId).ToScopeGroupFullName();
            await hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, monitorScopeGroup);
        }
    }
}

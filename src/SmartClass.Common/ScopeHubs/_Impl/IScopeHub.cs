using System;
using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs._Impl
{
    /// <summary>
    /// ScopedHub interface
    /// </summary>
    public interface IScopeHub
    {
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        Task OnConnectedAsync();
        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task OnDisconnectedAsync(Exception exception);

        /// <summary>
        /// 加入组
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task JoinGroup(JoinGroupArgs args);
        /// <summary>
        /// 从组移除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task LeaveGroup(LeaveGroupArgs args);

        /// <summary>
        /// 调用客户端方法
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task ClientMethod(ClientMethodArgs args);

        /// <summary>
        /// 重置Scope
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task ResetScope(ResetScopeArgs args);
        /// <summary>
        /// 更新Scope
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task UpdateScope(UpdateScopeArgs args);
    }
}

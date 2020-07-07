using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.Applications
{
    /// <summary>
    /// ScopedHub interface
    /// </summary>
    public interface IScopedHub
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
        Task AddToGroup(AddToGroup args);
        /// <summary>
        /// 从组移除
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task RemoveFromGroup(RemoveFromGroup args);

        /// <summary>
        /// 客户端方法自主调用后广播
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task ClientInvoke(ClientMethodArgs args);
        /// <summary>
        /// 调用客户端桩子方法（来自外部）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task ClientStub(ClientMethodArgs args);

        /// <summary>
        /// 踢掉Client
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task KickClient(KickClientArgs args);
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

    public class ResetScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }
    public class UpdateScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }
}
using System;
using System.Threading.Tasks;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopedHub.EventBus;
using SmartClass.Common.ScopedHub.Scopes;

namespace SmartClass.Common.ScopedHub.Applications
{
    public interface IScopedHub
    {
        SignalREventBus Bus { get; set; }

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
        /// 踢掉
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task KickClient(KickClientArgs args);

        /// <summary>
        /// 重置Scope
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task Reset(ScopeContext args);

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
    }
}
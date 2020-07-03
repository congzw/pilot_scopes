using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods.Stubs;
using SmartClass.Common.ScopedHub.EventBus;

namespace SmartClass.Common.ScopedHub.Applications
{
    //a demo for how to use ClientMonitors

    public class _AnyHub : Hub, IScopedHub
    {
        public _AnyHub(SignalREventBus hubEventBus)
        {
            Bus = hubEventBus;
        }

        public SignalREventBus Bus { get; set; }

        //连接
        public override async Task OnConnectedAsync()
        {
            await Bus.Raise(new OnConnectedEvent(this)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        //断开
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var reason = exception == null ? "" : exception.Message;
            await Bus.Raise(new OnDisconnectedEvent(this, reason)).ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }
        
        //踢掉（管理场景）
        public async Task KickClient(KickClientArgs args)
        {
            await Bus.Raise(new KickClientEvent(this, args)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        //重置Scope
        public Task Reset(ScopeContext args)
        {
            return Bus.Raise(new ResetEvent(this, args));
        }

        //加入组成员
        public Task AddToGroup(AddToGroup args)
        {
            return Bus.Raise(new AddToGroupEvent(this, args));
        }

        //移除组成员
        public Task RemoveFromGroup(RemoveFromGroup args)
        {
            return Bus.Raise(new RemoveFromGroupEvent(this, args));
        }

        //代表客户端的方法调用，供同步页面等场景使用
        public Task ClientInvoke(ClientMethodArgs args)
        {
            return Bus.Raise(new OnClientInvokeEvent(this, args));
        }

        //代表从服务器端的方法调用，供数据通知等场景使用
        public Task ClientStub(ClientMethodArgs args)
        {
            return Bus.Raise(new OnCallClientStubEvent(this, args));
        }
    }
}

using System;
using System.Collections.Generic;
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

    public interface IScopedHub
    {
        SignalREventBus Bus { get; set; }

        Task OnConnectedAsync();
        Task OnDisconnectedAsync(Exception exception);
        Task KickClient(KickClientArgs args);
        Task Reset(ScopeContext args);

        Task AddToGroup(AddToGroup args);
        Task RemoveFromGroup(RemoveFromGroup args);

        Task ClientInvoke(ClientMethodArgs args);
        Task ClientStub(ClientMethodArgs args);
    }



    public class _AnyHub : Hub
    {
        private readonly SignalREventBus _hubEventBus;

        public _AnyHub(SignalREventBus hubEventBus)
        {
            _hubEventBus = hubEventBus;
        }

        //连接
        public override async Task OnConnectedAsync()
        {
            await _hubEventBus.Raise(new OnConnectedEvent(this)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        //断开
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var reason = exception == null ? "" : exception.Message;
            await _hubEventBus.Raise(new OnDisconnectedEvent(this, reason)).ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }
        
        //踢掉（管理场景）
        public async Task KickClient(KickClientArgs args)
        {
            await _hubEventBus.Raise(new OnKickEvent(this, args)).ConfigureAwait(false);
            await base.OnConnectedAsync().ConfigureAwait(false);
        }
        
        //加入组成员
        public Task AddToGroup(AddToGroup args)
        {
            return _hubEventBus.Raise(new AddToGroupEvent(this, args));
        }

        //移除组成员
        public Task RemoveFromGroup(RemoveFromGroup args)
        {
            return _hubEventBus.Raise(new RemoveFromGroupEvent(this, args));
        }

        //Scope的上下文切换
        public Task ChangeScope(ScopeContext args)
        {
            return _hubEventBus.Raise(new OnResetEvent(this, args));
        }
        
        //代表客户端的方法调用，供同步页面等场景使用
        public Task ClientMethodInvoke(ClientMethodArgs args)
        {
            //this.Clients.All.SendAsync("", null);
            //this.Clients.All.SendCoreAsync("", null);
            return _hubEventBus.Raise(new OnClientInvokeEvent(this, args));
        }

        //代表从服务器端的方法调用，供数据通知等场景使用
        public Task InvokeClientStub(ClientMethodArgs args)
        {
            return _hubEventBus.Raise(new OnCallClientStubEvent(this, args));
        }
    }
}

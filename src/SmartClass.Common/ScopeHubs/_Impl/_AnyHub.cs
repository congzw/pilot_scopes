using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs._Impl
{
    //a demo for how to use IScopeHub
    public class _AnyHub : Hub, IScopeHub
    {
        public _AnyHub(SignalREventBus hubEventBus)
        {
            EventLogHelper.Resolve().Log(string.Format("[_AnyHub] {0} >>>>>>>> {1}", "CTOR", string.Empty));
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
        
        //重置scope
        public Task ResetScope(ResetScopeArgs args)
        {
            //todo: check auth
            this.FixScopeIdForArgs(args);
            TraceHubContext("ResetScope");
            return Bus.Raise(new ResetScopeEvent(this, args));
        }

        //更新scope
        public Task UpdateScope(UpdateScopeArgs args)
        {
            //todo: check auth
            this.FixScopeIdForArgs(args);
            TraceHubContext("UpdateScope");
            return Bus.Raise(new UpdateScopeEvent(this, args));
        }

        //加入组成员
        public Task JoinGroup(JoinGroupArgs args)
        {
            this.FixScopeIdForArgs(args);
            TraceHubContext("JoinGroup");
            return Bus.Raise(new JoinGroupEvent(this, args));
        }

        //移除组成员
        public Task LeaveGroup(LeaveGroupArgs args)
        {
            this.FixScopeIdForArgs(args);
            TraceHubContext("LeaveGroup");
            return Bus.Raise(new LeaveGroupEvent(this, args));
        }

        //调用客户端方法
        public Task ClientMethod(ClientMethodArgs args)
        {
            TraceHubContext("ClientMethod");
            return Bus.Raise(new ClientMethodEvent(this, args));
        }

        private void TraceHubContext(string method)
        {
            var invokeCountKey = "_InvokeCount_";
            var invokeCount = 1;
            if (Context.Items.ContainsKey(invokeCountKey))
            {
                invokeCount = (int)Context.Items[invokeCountKey];
                invokeCount++;
            }
            this.Context.Items[invokeCountKey] = invokeCount;
            EventLogHelper.Resolve().Log(string.Format("[_AnyHub] {0} >>>>>>>> {1}", method, this.TryGetHttpContext().Request.QueryString));
        }
    }
}

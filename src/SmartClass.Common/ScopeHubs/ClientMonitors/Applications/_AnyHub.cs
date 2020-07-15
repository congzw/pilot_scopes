﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.Scopes;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Applications
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
        
        //客户端主动调用的方法，可用于双向通讯场景
        public Task ClientInvoke(ClientMethodArgs args)
        {
            TraceHubContext("ClientInvoke");
            return Bus.Raise(new ClientInvokeEvent(this, args));
        }

        //客户端被动调用的方法，可用于单向通讯场景
        public Task ClientStub(ClientMethodArgs args)
        {
            TraceHubContext("ClientStub");
            return Bus.Raise(new ClientStubEvent(this, args));
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

            //基于hub的所有方法调用，都能拿到基于当前的上下文信息参数Query
            //[13704] [_AnyHub] OnConnectedAsync >>>>>>>> ?scopeId=s1&clientId=c2&id=gMop-YWYX7zbRWdqJOhyig
            //[13704] [_AnyHub] OnDisconnectedAsync >>>>>>>> ?scopeId=s1&clientId=c2&id=gMop-YWYX7zbRWdqJOhyig
            //[10664] [_AnyHub] KickClient >>>>>>>> ?scopeId=s1&clientId=c1&id=hB1kQwNfvF9bu-Tp_cSaig
            //[10664] [_AnyHub] InvokeClientStub >>>>>>>> ?scopeId=s1&clientId=c1&id=hB1kQwNfvF9bu-Tp_cSaig
            EventLogHelper.Resolve().Log(string.Format("[_AnyHub] {0} >>>>>>>> {1}", method, this.TryGetHttpContext().Request.QueryString));
        }
    }
}

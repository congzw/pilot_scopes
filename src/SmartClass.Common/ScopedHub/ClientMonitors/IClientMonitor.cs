using System.Collections.Generic;
using System.Threading.Tasks;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientGroups;
using SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods;
using SmartClass.Common.ScopedHub.ClientMonitors.Groups;

namespace SmartClass.Common.ScopedHub.ClientMonitors
{
    public interface IClientMonitor : 
        IClientConnectionManager, 
        IScopeGroupManager, 
        IClientGroupManager, 
        IClientMethodManager
    {
    }

    public class ClientMonitor : IClientMonitor
    {
        public Task OnConnected(OnConnectedEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task OnDisconnected(OnDisconnectedEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task KickClient(KickClientEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task ResetScope(ResetScopeEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateScope(UpdateScopeEvent theEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<ScopeGroup>> GetGroups(IScopeGroupLocate args)
        {
            throw new System.NotImplementedException();
        }

        public Task<ScopeGroup> GetGroup(IScopeGroupLocate args)
        {
            throw new System.NotImplementedException();
        }

        public Task AddGroup(AddGroup args)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveGroup(RemoveGroup args)
        {
            throw new System.NotImplementedException();
        }

        public Task AddToGroup(AddToGroup args)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveFromGroup(RemoveFromGroup args)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<ScopeClientGroup>> GetGroups(IScopeClientGroupLocate args)
        {
            throw new System.NotImplementedException();
        }

        public Task ClientInvoke(ClientMethodArgs args)
        {
            throw new System.NotImplementedException();
        }

        public Task ClientStub(ClientMethodArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}

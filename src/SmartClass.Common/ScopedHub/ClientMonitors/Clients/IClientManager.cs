using System.Collections.Generic;

namespace SmartClass.Common.ScopedHub.ClientMonitors.Clients
{
    public interface IClientManager
    {
        void Connect(IScopeClientLocate locate);
        void Disconnect(IScopeClientLocate locate);
        void Kick(IScopeClientLocate locate);
        void Update(ClientContext locate);
    }

    public class ClientContext : IScopeClientLocate, IHaveBags
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }
}

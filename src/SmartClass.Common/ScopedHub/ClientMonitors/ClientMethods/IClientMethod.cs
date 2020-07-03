using System.Collections.Generic;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods
{
    public interface IClientMethod : IHaveBags
    {
        string Method { get; set; }
    }

    public class ClientMethodArgs : IClientMethod
    {
        public ClientMethodArgs()
        {
            Bags = BagsHelper.Create();
            ToClientIds = new List<string>();
            ToGroups = new List<string>();
        }

        public IDictionary<string, object> Bags { get; set; }
        public string Method { get; set; }

        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ComponentId { get; set; }
        public List<string> ToClientIds { get; set; }
        public List<string> ToGroups { get; set; }
    }
}

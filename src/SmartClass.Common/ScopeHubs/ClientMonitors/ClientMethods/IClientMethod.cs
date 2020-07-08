using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethod : IHaveBags
    {
        string Method { get; set; }
    }

    public class ClientMethodArgs : IClientMethod, IScopeClientLocate
    {
        public ClientMethodArgs()
        {
            Bags = BagsHelper.Create();
            ToClientIds = new List<string>();
            ToGroups = new List<string>();
        }
        
        public string ScopeId { get; set; }
        public string Method { get; set; }
        public object MethodArgs { get; set; }
        public IDictionary<string, object> Bags { get; set; }

        public string ClientId { get; set; }
        public string ComponentId { get; set; }
        public List<string> ToClientIds { get; set; }
        public List<string> ToGroups { get; set; }

        public static ClientMethodArgs Create(string method)
        {
            return new ClientMethodArgs(){Method = method};
        }
    }
}

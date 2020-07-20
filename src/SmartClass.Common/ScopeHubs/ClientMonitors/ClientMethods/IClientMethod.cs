using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethod
    {
        string Method { get; set; }
        object MethodArgs { get; set; }
    }

    public class ClientMethodArgs : IClientMethod
    {

        public string Method { get; set; }
        public object MethodArgs { get; set; }
        public SendContext SendContext { get; set; } = new SendContext();

        public static ClientMethodArgs Create()
        {
            return new ClientMethodArgs();
        }

        public static ClientMethodArgs Create(string method)
        {
            return new ClientMethodArgs { Method = method };
        }
    }

    public static class ClientMethodExtensions
    {
        public static ClientMethodArgs ForNotify(this ClientMethodArgs self, object methodArgs)
        {
            self.Method = HubConst.ClientMethod_StubNotify;
            self.MethodArgs = methodArgs;
            return self;
        }

        public static ClientMethodArgs ForKicked(this ClientMethodArgs self, object methodArgs)
        {
            self.Method = HubConst.ClientMethod_StubKicked;
            self.MethodArgs = methodArgs;
            return self;
        }

        private static string ClientMethod = "clientMethod";
        public static Task SendAsyncToClientMethod(this IClientProxy clientProxy, ClientMethodArgs args)
        {
            return clientProxy.SendAsync(ClientMethod, args);
        }
    }
}

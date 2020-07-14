using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethod : IHaveBags
    {
        string Method { get; set; }
        object MethodArgs { get; set; }
    }

    public class ClientMethodArgs : IClientMethod
    {
        public ClientMethodArgs()
        {
            Bags = BagsHelper.Create();
        }
        
        public string Method { get; set; }
        public object MethodArgs { get; set; }
        public IDictionary<string, object> Bags { get; set; }

        public static ClientMethodArgs Create(string method)
        {
            return new ClientMethodArgs {Method = method};
        }
    }

    public static class ClientMethodArgsExtensions
    {
        public static void WithSendContext<T>(this T args, SendContext sendContext) where T : IClientMethod
        {
            args.SetBagValue("SendContext", sendContext);
        }
    }
}

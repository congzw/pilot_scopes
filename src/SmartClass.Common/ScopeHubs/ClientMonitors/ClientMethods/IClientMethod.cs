using System.Collections.Generic;

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

    public static class ClientMethodArgsExtensions
    {
        //public static void WithSendContext<T>(this T args, SendContext sendContext) where T : IClientMethod
        //{
        //    args.SetBagValue("SendContext", sendContext);
        //}

        public static ClientMethodArgs ForLogMessage(this ClientMethodArgs self, object methodArgs)
        {
            self.Method = HubConst.ClientMethod_LogMessage;
            self.MethodArgs = methodArgs;
            return self;
        }

        public static ClientMethodArgs ForNotify(this ClientMethodArgs self, object methodArgs)
        {
            self.Method = HubConst.ClientMethod_Notify;
            self.MethodArgs = methodArgs;
            return self;
        }

        public static ClientMethodArgs ForKicked(this ClientMethodArgs self, object methodArgs)
        {
            self.Method = HubConst.ClientMethod_Kicked;
            self.MethodArgs = methodArgs;
            return self;
        }
    }
}

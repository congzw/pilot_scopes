using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethod
    {
        string Method { get; set; }
        IDictionary<string, object> MethodArgs { get; set; }
    }

    public class ClientMethodArgs : IClientMethod
    {
        public string Method { get; set; }
        public IDictionary<string, object> MethodArgs { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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
            self.MethodArgs = methodArgs.ToDictionary();
            return self;
        }

        public static ClientMethodArgs ForKicked(this ClientMethodArgs self, object methodArgs)
        {
            self.Method = HubConst.ClientMethod_StubKicked;
            self.MethodArgs = methodArgs.ToDictionary();
            return self;
        }

        public static Task SendAsyncToClientMethod(this IClientProxy clientProxy, ClientMethodArgs args)
        {
            return clientProxy.SendAsync(HubConst.OnClientMethod, args);
        }

        public static TValue GetArgsValue<TValue>(this IClientMethod clientMethod, string key, TValue defaultValue)
        {
            var dictionary = clientMethod.MethodArgs.ToDictionary();
            var theValue = dictionary.TryGetValueAs(key, defaultValue);
            return theValue;
        }

        public static void SetArgsValue(this IClientMethod clientMethod, string key, object value)
        {
            var dictionary = clientMethod.MethodArgs;
            dictionary[key] = value;
        }
    }
}

using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethodManager
    {
        /// <summary>
        /// 客户端方法的主动调用事件: Client -> Server -> Client
        /// </summary>
        /// <param name="theEvent"></param>
        /// <returns></returns>
        Task ClientInvoke(ClientInvokeEvent theEvent);

        /// <summary>
        /// 客户端桩子方法的被动调用事件: ? -> Client
        /// </summary>
        /// <param name="theEvent"></param>
        /// <returns></returns>
        Task ClientStub(ClientStubEvent theEvent);
    }
}
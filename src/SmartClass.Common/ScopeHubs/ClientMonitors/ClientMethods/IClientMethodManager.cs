using System.Threading.Tasks;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Invokes;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethodManager
    {
        //客户端方法的主动调用事件
        Task ClientInvoke(ClientInvokeEvent theEvent);

        //客户端桩子方法的被动调用事件
        Task ClientStub(ClientStubEvent theEvent);
    }
}
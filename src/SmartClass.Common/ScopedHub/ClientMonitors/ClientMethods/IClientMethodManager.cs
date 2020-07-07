using System.Threading.Tasks;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods
{
    public interface IClientMethodManager
    {
        Task ClientInvoke(ClientMethodArgs args);
        Task ClientStub(ClientMethodArgs args);
    }
}
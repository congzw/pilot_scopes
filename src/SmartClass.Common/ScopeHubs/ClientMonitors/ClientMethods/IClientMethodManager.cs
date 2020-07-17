using System.Threading.Tasks;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    public interface IClientMethodManager
    {
        /// <summary>
        /// 客户端方法的调用事件
        /// </summary>
        /// <param name="theEvent"></param>
        /// <returns></returns>
        Task ClientMethod(ClientMethodEvent theEvent);
    }
}
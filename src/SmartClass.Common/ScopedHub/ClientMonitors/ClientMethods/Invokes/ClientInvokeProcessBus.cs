using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientMethods.Invokes
{
    /// <summary>
    /// 服务器端的处理逻辑
    /// </summary>
    public interface IClientInvokeProcess : IMyScoped
    {
        float ProcessOrder { set; get; }
        bool ShouldProcess(OnClientInvokeEvent @event);
        Task ProcessAsync(OnClientInvokeEvent @event);
    }

    public class ClientInvokeProcessBus : IMyScoped
    {
        public IEnumerable<IClientInvokeProcess> Processes { get; }

        public ClientInvokeProcessBus(IEnumerable<IClientInvokeProcess> processes)
        {
            Processes = processes;
        }

        public async Task Process(OnClientInvokeEvent @event)
        {
            if (@event == null)
            {
                return;
            }
            var sortedProcesses = Processes
                .Where(x => x.ShouldProcess(@event))
                .OrderBy(x => x.ProcessOrder)
                .ToList();

            foreach (var sortedProcess in sortedProcesses)
            {
                try
                {
                    //todo trace log
                    await sortedProcess.ProcessAsync(@event).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    //todo log
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}

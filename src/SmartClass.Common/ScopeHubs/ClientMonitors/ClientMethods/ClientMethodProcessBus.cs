using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods
{
    /// <summary>
    /// 服务器端的干预逻辑
    /// </summary>
    public interface IClientMethodProcess : IMyScoped
    {
        float ProcessOrder { get; set; }
        bool ShouldProcess(ClientMethodEvent @event);
        Task ProcessAsync(ClientMethodEvent @event);
    }

    public class ClientMethodProcessBus : IMyScoped
    {
        public IEnumerable<IClientMethodProcess> Processes { get; }
        public ClientMethodProcessBus(IEnumerable<IClientMethodProcess> processes)
        {
            Processes = processes;
        }

        public async Task Process(ClientMethodEvent @event)
        {
            if (@event == null || Processes == null)
            {
                return;
            }

            var sortedProcesses = Processes
                .Where(x => x.ShouldProcess(@event))
                .OrderBy(x => x.ProcessOrder)
                .ToList();

            foreach (var sortedProcess in sortedProcesses)
            {
                await sortedProcess.ProcessAsync(@event).ConfigureAwait(false);
            }
        }
    }
}
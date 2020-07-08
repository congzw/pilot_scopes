using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.ClientMethods.Stubs
{
    public interface IClientStubProcess : IMyScoped
    {
        float ProcessOrder { get; set; }
        bool ShouldProcess(ClientStubEvent @event);
        Task ProcessAsync(ClientStubEvent @event);
    }

    public class ClientStubProcessBus : IMyScoped
    {
        public IEnumerable<IClientStubProcess> Processes { get; }
        public ClientStubProcessBus(IEnumerable<IClientStubProcess> processes)
        {
            Processes = processes;
        }
        public async Task Process(ClientStubEvent @event)
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

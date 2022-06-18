using System.Threading;
using System.Threading.Tasks;
using MassTransit.SagaStateMachine;
using Microsoft.Extensions.Hosting;
using MassTransit.Visualizer;

namespace PermitService
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var permitRequestStateMachine = new PermitRequestStateMachine(null);
            var graph = permitRequestStateMachine.GetGraph();
            var file = new StateMachineGraphvizGenerator(graph).CreateDotFile();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace DefaultValues
{
    public class Worker : BackgroundService
    {
        private readonly IBus bus;

        public Worker(IBus bus)
        {
            this.bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await bus.Send(new User
            {
                Name = default(string),
                IsRegistered = default(bool)
            }, cancellationToken: stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
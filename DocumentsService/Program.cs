using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Entities.Commands;
using DocumentsService.Consumers;

namespace DocumentsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();

                    services.AddMassTransit(config =>
                    {
                        config.UsingRabbitMq((context, rabbitConfig) =>
                        {
                            rabbitConfig.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            rabbitConfig.ConfigureEndpoints(context);
                        });

                        EndpointConvention.Map<AddDocuments>(new Uri($"queue:{nameof(AddDocuments)}"));

                        config.AddConsumer<AddDocumentsConsumer>();
                    });
                })
                .Build();

            host.Run();
        }
    }
}
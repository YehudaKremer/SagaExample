using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Entities.Commands;
using ParticipantsService.Consumers;

namespace ParticipantsService
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

                        EndpointConvention.Map<AddParticipants>(new Uri($"queue:{nameof(AddParticipants)}"));
                        EndpointConvention.Map<RemoveParticipants>(new Uri($"queue:{nameof(RemoveParticipants)}"));

                        config.AddConsumer<AddParticipantsConsumer>();
                        config.AddConsumer<RemoveParticipantsConsumer>();
                    });
                })
                .Build();

            host.Run();
        }
    }
}
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using System.Reflection;
using DefaultValues.Consumers;

namespace DefaultValues
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {

                    services.AddMassTransit(config =>
                    {
                        config.UsingRabbitMq((context, rabbitMqConfig) =>
                        {
                            //rabbitMqConfig.UseMessageRetry(i => i.Immediate(3));

                            rabbitMqConfig.ConfigureEndpoints(context);

                            rabbitMqConfig.ReceiveEndpoint("User", e =>
                            {
                                e.UseMessageRetry(r => r.Immediate(5));
                                e.Consumer(() => new UserConsumer());
                            });
                        });

                        EndpointConvention.Map<User>(new Uri("queue:User"));



                        //config.AddConsumers(Assembly.GetEntryAssembly());
                    });

                    services.AddOptions<MassTransitHostOptions>().Configure(config =>
                    {
                        config.WaitUntilStarted = true;
                    });

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}
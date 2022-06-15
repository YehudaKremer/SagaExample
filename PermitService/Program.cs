using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Entities.Commands;
using Entities.Events;

namespace PermitService
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
                        config.UsingRabbitMq((context, rabbitConfig) =>
                        {
                            rabbitConfig.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            rabbitConfig.ConfigureEndpoints(context);
                        });

                        EndpointConvention.Map<CreatePermitRequest>(new Uri($"queue:{nameof(CreatePermitRequest)}"));
                        EndpointConvention.Map<AddParticipants>(new Uri($"queue:{nameof(AddParticipants)}"));
                        EndpointConvention.Map<RemoveParticipants>(new Uri($"queue:{nameof(RemoveParticipants)}"));
                        EndpointConvention.Map<AddDocuments>(new Uri($"queue:{nameof(AddDocuments)}"));

                        GlobalTopology.Send.UseCorrelationId<AddDocuments>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<AddParticipants>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<CreatePermitRequest>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<RemoveParticipants>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<DocumentsAdded>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<DocumentRejected>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<ParticipantsAdded>(i => i.PermitRequest.PermitRequestId);
                        GlobalTopology.Send.UseCorrelationId<ParticipantsRemoved>(i => i.PermitRequest.PermitRequestId);

                        config.AddSagaStateMachine<PermitRequestStateMachine, PermitRequestState>()
                            .InMemoryRepository();
                        //.DapperRepository(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SagaExample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False");
                    });

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}
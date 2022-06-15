using System;
using Microsoft.Extensions.Logging;
using MassTransit;
using Dapper.Contrib.Extensions;
using Entities.Commands;
using Entities.Events;

namespace PermitService
{
    public class PermitRequestState : SagaStateMachineInstance
    {
        [ExplicitKey]
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
    }
    public class PermitRequestStateMachine : MassTransitStateMachine<PermitRequestState>
    {
        // Custom statuses for this saga
        public State AddingParticipants { get; private set; }
        public State AddingDocuments { get; private set; }
        public State RemovingParticipants { get; private set; }

        // Events in this saga
        public Event<CreatePermitRequest> CreatePermitRequest { get; private set; }
        public Event<ParticipantsAdded> ParticipantsAdded { get; private set; }
        public Event<ParticipantsRemoved> ParticipantsRemoved { get; private set; }
        public Event<DocumentsAdded> DocumentsAdded { get; private set; }
        public Event<DocumentRejected> DocumentRejected { get; private set; }

        public PermitRequestStateMachine(ILogger<PermitRequestStateMachine> logger)
        {
            InstanceState(x => x.CurrentState, AddingParticipants, RemovingParticipants, AddingDocuments);

            Initially(
                When(CreatePermitRequest)
                    .Send(context => new AddParticipants { PermitRequest = context.Message.PermitRequest })
                    .TransitionTo(AddingParticipants));

            During(AddingParticipants,
                When(ParticipantsAdded)
                    .Send(context => new AddDocuments { PermitRequest = context.Message.PermitRequest })
                    .TransitionTo(AddingDocuments));

            // Rollback participants
            During(AddingDocuments,
                When(DocumentRejected)
                    .Send(context => new RemoveParticipants { PermitRequest = context.Message.PermitRequest })
                    .TransitionTo(RemovingParticipants));

            DuringAny(
                When(DocumentsAdded)
                    .Then(context => logger.LogInformation("PermitService -> PermitRequestStateMachine: Saga finish successfully, correlation id: {id}",
                        context.CorrelationId)),
                When(ParticipantsRemoved)
                    .Then(context => logger.LogInformation("PermitService -> PermitRequestStateMachine: Saga finish with rollback, correlation id: {id}",
                        context.CorrelationId))
                .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}

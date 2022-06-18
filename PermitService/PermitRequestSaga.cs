using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MassTransit;
using Dapper.Contrib.Extensions;
using Entities.Commands;
using Entities.Events;
using Entities.Models;

namespace PermitService
{
    public class PermitRequestState : SagaStateMachineInstance
    {
        [ExplicitKey]
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public List<Document> Documents { get; set; }
        public List<Participant> SavedParticipants { get; set; }
    }

    public class PermitRequestStateMachine : MassTransitStateMachine<PermitRequestState>
    {
        // Custom statuses for this saga
        public State AddingParticipants { get; private set; }
        public State AddingDocuments { get; private set; }
        public State RemovingParticipants { get; private set; }

        // Events/Commands used in this saga
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
                    .Then(c => c.Saga.Documents = c.Message.PermitRequest.Documents)
                    .Send(c => new AddParticipants(c.Saga.CorrelationId, c.Message.PermitRequest.Participants))
                    .TransitionTo(AddingParticipants));

            During(AddingParticipants,
                When(ParticipantsAdded)
                    .Then(c => c.Saga.SavedParticipants = c.Message.Participants)
                    .Send(c => new AddDocuments(c.Saga.CorrelationId, c.Saga.Documents))
                    .TransitionTo(AddingDocuments));

            During(AddingDocuments,
                When(DocumentsAdded)
                    .Then(context => logger.LogInformation("PermitService -> PermitRequestStateMachine: Saga finish successfully, correlation id: {id}",
                        context.CorrelationId))
                        .Finalize(),
                When(DocumentRejected) // If document rejected the rollback and delete saved participants
                    .Send(c => new RemoveParticipants(c.Saga.CorrelationId, c.Saga.SavedParticipants))
                    .TransitionTo(RemovingParticipants));

            During(RemovingParticipants,
                When(ParticipantsRemoved)
                    .Then(context => logger.LogInformation("PermitService -> PermitRequestStateMachine: Saga finish with rollback, correlation id: {id}",
                        context.CorrelationId))
                .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}

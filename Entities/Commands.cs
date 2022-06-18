using System;
using System.Collections.Generic;
using Entities.Models;

namespace Entities.Commands
{
    public record CreatePermitRequest(PermitRequest PermitRequest);
    public record AddParticipants(Guid CorrelationId, List<Participant> Participants);
    public record AddDocuments(Guid CorrelationId, List<Document> Documents);
    public record RemoveParticipants(Guid CorrelationId, List<Participant> Participants);
}

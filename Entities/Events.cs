using System;
using System.Collections.Generic;
using Entities.Models;

namespace Entities.Events
{
    public record ParticipantsAdded(Guid CorrelationId, List<Participant> Participants);
    public record DocumentsAdded(Guid CorrelationId);
    public record DocumentRejected(Guid CorrelationId);
    public record ParticipantsRemoved(Guid CorrelationId);
}

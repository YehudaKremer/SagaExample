using System;
using System.Collections.Generic;
using MassTransit;

namespace Entities.Models
{
    public class PermitRequest
    {
        public Guid PermitRequestId { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Document> Documents { get; set; }
    }
}

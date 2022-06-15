using Entities.Models;

namespace Entities.Events
{
    public class DocumentRejected
    {
        public PermitRequest PermitRequest { get; set; }
    }
}

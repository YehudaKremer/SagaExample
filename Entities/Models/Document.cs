using System;

namespace Entities.Models
{
    public class Document
    {
        public Guid DocumentID { get; set; }
        public string Name { get; set; }
        public bool IsValidDocument { get; set; }
    }
}

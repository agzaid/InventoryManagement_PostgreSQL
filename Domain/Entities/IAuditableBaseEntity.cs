using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public interface IAuditableBaseEntity
    {
        // Must match the property names exactly
        public DateTime DateCreated { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}

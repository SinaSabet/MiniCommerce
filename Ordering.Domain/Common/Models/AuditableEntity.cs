using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Common.Models
{
    public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
    {
        public DateTime CreatedAt { get; private set; }


        public DateTime? UpdatedAt { get; private set; }

        protected AuditableEntity(TId id) : base(id)
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

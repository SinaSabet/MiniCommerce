using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Common.Models
{
    public abstract class Entity<TId> where TId : notnull 
    {
        public TId Id { get; set; }

        protected Entity(TId id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;


            if (obj.GetType() != GetType())
                return false;


            var entity = (Entity<TId>)obj;


            return EqualityComparer<TId>
                .Default
                .Equals(Id, entity.Id);
        }

        public static bool operator ==(
     Entity<TId>? left,
     Entity<TId>? right)
        {
            if (ReferenceEquals(left, right))
                return true;


            if (left is null || right is null)
                return false;


            return left.Equals(right);
        }



        public static bool operator !=(
            Entity<TId>? left,
            Entity<TId>? right)
        {
            return !(left == right);
        }



        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Common.Models
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object>
       GetEqualityComponents();

        public override bool Equals(object? obj)
        {

            if (obj is null)
                return false;


            if (obj.GetType() != GetType())
                return false;



            var valueObject =
                (ValueObject)obj;



            return GetEqualityComponents()
                .SequenceEqual(
                    valueObject.GetEqualityComponents()
                );

        }



        public static bool operator ==(
            ValueObject? left,
            ValueObject? right)
        {

            if (ReferenceEquals(left, right))
                return true;


            if (left is null || right is null)
                return false;


            return left.Equals(right);
        }



        public static bool operator !=(
            ValueObject? left,
            ValueObject? right)
        {
            return !(left == right);
        }



        public override int GetHashCode()
        {

            return GetEqualityComponents()
                .Aggregate(
                    0,
                    (current, obj) =>
                    HashCode.Combine(
                        current,
                        obj.GetHashCode()
                    ));

        }


    }
}

using Ordering.Domain.Common.Exceptions;
using Ordering.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public sealed class OrderNumber : ValueObject
    {
        public string Value { get; private set; }

        private OrderNumber() { }
        public OrderNumber(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(
                    "Order number required");


            Value = value;

        }
        public static OrderNumber Create()
        {

            var number =
                $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}";


            return new OrderNumber(number);

        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public override string ToString()
        {
            return Value;
        }
    }
}

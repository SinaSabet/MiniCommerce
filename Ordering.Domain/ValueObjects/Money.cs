using Ordering.Domain.Common.Exceptions;
using Ordering.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }


        public string Currency { get; private set; }


        public Money(
       decimal amount,
       string currency)
        {

            if (amount < 0)
                throw new DomainException(
                    "Money cannot be negative");


            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException(
                    "Currency is required");


            Amount = amount;
            Currency = currency.ToUpper();

        }
        private Money()
        {

        }
        public static Money Zero(
      string currency = "IRR")
        {
            return new Money(0, currency);
        }


        public Money Add(Money other)
        {

            if (Currency != other.Currency)
                throw new DomainException(
                    "Currency mismatch");


            return new Money(
                Amount + other.Amount,
                Currency);
        }

        public static Money operator +(
    Money left,
    Money right)
        {

            if (left.Currency != right.Currency)
                throw new DomainException(
                "Currency mismatch");


            return new Money(
                left.Amount + right.Amount,
                left.Currency);
        }

        public static Money operator *(
    Money money,
    int quantity)
        {

            return new Money(
                money.Amount * quantity,
                money.Currency);
        }
        public Money Multiply(int quantity)
        {

            if (quantity < 0)
                throw new DomainException(
                    "Invalid quantity");


            return new Money(
                Amount * quantity,
                Currency);
        }



        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}

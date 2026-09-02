using Payment.Domain.Common.Exceptions;
using Payment.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.ValueObjects
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
      string currency = "USD")
        {
            return new Money(0, currency);
        }


        public Money Add(Money other)
        {

            if (other is null)
                throw new DomainException(
                    "Money cannot be null");


            if (Currency != other.Currency)
                throw new DomainException(
                    "Currency must be the same");


            return new Money(
                Amount + other.Amount,
                Currency);
        }


        public Money Subtract(Money other)
        {

            if (other is null)
                throw new DomainException(
                    "Money cannot be null");


            if (Currency != other.Currency)
                throw new DomainException(
                    "Currency must be the same");


            if (Amount < other.Amount)
                throw new DomainException(
                    "Insufficient amount");


            return new Money(
                Amount - other.Amount,
                Currency);
        }

        protected override IEnumerable<object>
       GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}

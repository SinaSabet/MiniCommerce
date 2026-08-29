using Ordering.Domain.Common.Exceptions;
using Ordering.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public class Address:ValueObject
    {
        public string City { get; private set; }


        public string Street { get; private set; }


        public string PostalCode { get; private set; }

        private Address()
        {

        }

        public Address(
       string city,
       string street,
       string postalCode)
        {

            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException(
                    "City required");


            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException(
                    "Street required");


            City = city;
            Street = street;
            PostalCode = postalCode;

        }
        protected override IEnumerable<object>
     GetEqualityComponents()
        {
            yield return City;
            yield return Street;
            yield return PostalCode;
        }
    }
}

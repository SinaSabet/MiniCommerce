using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {

            RuleFor(x => x.ProductId)
                .NotEmpty();


            RuleFor(x => x.Quantity)
                .GreaterThan(0);


            RuleFor(x => x.Price)
                .GreaterThan(0);


            RuleFor(x => x.Currency)
                .NotEmpty();


            RuleFor(x => x.ShippingAddress.City)
                .NotEmpty();


            RuleFor(x => x.ShippingAddress.Street)
                .NotEmpty();

        }
    }
}

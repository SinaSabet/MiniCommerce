using Inventory.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>
     : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;



        public TransactionBehavior(
            IUnitOfWork unitOfWork,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }



        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {

            await _unitOfWork
                .BeginTransactionAsync(
                    cancellationToken);



            try
            {

                var response =
                    await next();


                await _unitOfWork
                    .DispatchDomainEventsAsync(
                        cancellationToken);


                await _unitOfWork
                    .SaveChangesAsync(
                        cancellationToken);



                await _unitOfWork
                    .CommitTransactionAsync(
                        cancellationToken);


                _unitOfWork.ClearDomainEvents();


                _logger.LogInformation(
                    "Transaction committed successfully for {RequestName}",
                    typeof(TRequest).Name);



                return response;

            }
            catch (Exception ex)
            {

                await _unitOfWork
                    .RollbackTransactionAsync(
                        cancellationToken);



                _logger.LogError(
                    ex,
                    "Transaction rolled back for {RequestName}",
                    typeof(TRequest).Name);



                throw;

            }

        }

    }
}

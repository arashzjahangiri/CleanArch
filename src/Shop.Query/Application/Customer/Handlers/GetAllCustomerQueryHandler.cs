using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Handlers;

public class GetAllCustomerQueryHandler(
    IValidator<GetAllCustomerQuery> validator,
    ICustomerReadOnlyRepository repository)
    : IRequestHandler<GetAllCustomerQuery, Result<PagedQueryResult<CustomerQueryModel>>>
{
    public async Task<Result<PagedQueryResult<CustomerQueryModel>>> Handle(
        GetAllCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PagedQueryResult<CustomerQueryModel>>.Invalid(validationResult.AsErrors());
        }

        // Deliberately uncached: a page of a mutable list cannot be invalidated precisely,
        // so caching it would serve stale pages after any customer change.
        var page = await repository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PagedQueryResult<CustomerQueryModel>>.Success(page);
    }
}

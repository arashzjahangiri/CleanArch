using Ardalis.Result;
using MediatR;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Queries;

public class GetAllCustomerQuery(int pageNumber = 1, int pageSize = 20)
    : IRequest<Result<PagedQueryResult<CustomerQueryModel>>>
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
}

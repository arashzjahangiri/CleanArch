using System;
using System.Threading;
using System.Threading.Tasks;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories.Abstractions;

public interface ICustomerReadOnlyRepository : IReadOnlyRepository<CustomerQueryModel, Guid>
{
    /// <summary>
    /// Reads one page of customers, ordered by first name then date of birth.
    /// </summary>
    Task<PagedQueryResult<CustomerQueryModel>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

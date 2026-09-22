using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

internal class CustomerReadOnlyRepository(IReadDbContext readDbContext)
    : BaseReadOnlyRepository<CustomerQueryModel, Guid>(readDbContext), ICustomerReadOnlyRepository
{
    public async Task<PagedQueryResult<CustomerQueryModel>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<CustomerQueryModel>.Filter.Empty;

        var sort = Builders<CustomerQueryModel>.Sort
            .Ascending(customer => customer.FirstName)
            .Descending(customer => customer.DateOfBirth);

        var findOptions = new FindOptions<CustomerQueryModel>
        {
            Sort = sort,
            Skip = (pageNumber - 1) * pageSize,
            Limit = pageSize
        };

        var totalCount = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        using var asyncCursor = await Collection.FindAsync(filter, findOptions, cancellationToken);
        var items = await asyncCursor.ToListAsync(cancellationToken);

        return new PagedQueryResult<CustomerQueryModel>(items, pageNumber, pageSize, totalCount);
    }
}

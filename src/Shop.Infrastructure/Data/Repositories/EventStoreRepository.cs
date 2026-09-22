using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

internal sealed class EventStoreRepository(EventStoreDbContext dbContext) : IEventStoreRepository
{
    public async Task StoreAsync(IEnumerable<EventStore> eventStores, CancellationToken cancellationToken = default)
    {
        await dbContext.EventStores.AddRangeAsync(eventStores, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
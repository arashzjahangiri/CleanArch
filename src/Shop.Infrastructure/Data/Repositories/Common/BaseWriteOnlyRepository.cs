using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories.Common;

/// <summary>
/// Base class for write-only repositories.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
internal abstract class BaseWriteOnlyRepository<TEntity, TKey>(WriteDbContext dbContext) : IWriteOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private static readonly Func<WriteDbContext, TKey, CancellationToken, Task<TEntity>> GetByIdCompiledAsync =
        EF.CompileAsyncQuery((WriteDbContext dbContext, TKey id, CancellationToken cancellationToken) =>
            dbContext
                .Set<TEntity>()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(entity => entity.Id.Equals(id)));

    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
    protected readonly WriteDbContext DbContext = dbContext;

    public void Add(TEntity entity) =>
        _dbSet.Add(entity);

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void Remove(TEntity entity) =>
        _dbSet.Remove(entity);

    public async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default) =>
        await GetByIdCompiledAsync(DbContext, id, cancellationToken);

}
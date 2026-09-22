using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Repositories.Common;

namespace Shop.Infrastructure.Data.Repositories;

internal class CustomerWriteOnlyRepository(WriteDbContext dbContext)
    : BaseWriteOnlyRepository<Customer, Guid>(dbContext), ICustomerWriteOnlyRepository
{
    private static readonly Func<WriteDbContext, string, CancellationToken, Task<bool>> ExistsByEmailCompiledAsync =
        EF.CompileAsyncQuery((WriteDbContext dbContext, string email, CancellationToken cancellationToken) =>
            dbContext
                .Customers
                .AsNoTracking()
                .Any(customer => customer.Email.Address == email));

    private static readonly Func<WriteDbContext, string, Guid, CancellationToken, Task<bool>> ExistsByEmailAndIdCompiledAsync =
        EF.CompileAsyncQuery((WriteDbContext dbContext, string email, Guid currentId, CancellationToken cancellationToken) =>
            dbContext
                .Customers
                .AsNoTracking()
                .Any(customer => customer.Email.Address == email && customer.Id != currentId));

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        ExistsByEmailCompiledAsync(DbContext, email.Address, cancellationToken);

    public Task<bool> ExistsByEmailAsync(Email email, Guid currentId, CancellationToken cancellationToken = default) =>
        ExistsByEmailAndIdCompiledAsync(DbContext, email.Address, currentId, cancellationToken);
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Infrastructure.Data.Context;
using Shop.Query.Abstractions;

namespace Shop.PublicApi.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task RunAppAsync(this WebApplication app)
    {
        await using var serviceScope = app.Services.CreateAsyncScope();

        // Migrate the databases asynchronously using the provided service scope
        await app.MigrateDataBasesAsync(serviceScope);

        app.Logger.LogInformation("----- Application is starting....");

        await app.RunAsync();
    }

    private static async Task MigrateDataBasesAsync(this WebApplication app, AsyncServiceScope serviceScope)
    {
        // Resolved from the scope, so the scope disposes them. Disposing here as well would
        // return a pooled context to the pool twice.
        var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
        var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        var readDbContext = serviceScope.ServiceProvider.GetRequiredService<IReadDbContext>();

        try
        {
            await app.MigrateDbContextAsync(writeDbContext);
            await app.MigrateDbContextAsync(eventStoreDbContext);
            await app.MigrateMongoDbContextAsync(readDbContext);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An exception occurred while initializing the application: {Message}", ex.Message);
            throw;
        }
    }

    private static async Task MigrateDbContextAsync<TDbContext>(this WebApplication app, TDbContext dbContext)
        where TDbContext : DbContext
    {
        var dbName = dbContext.Database.GetDbConnection().Database;

        app.Logger.LogInformation("----- {DbName}: checking if there are any pending migrations...", dbName);

        // Migrations the database has not applied yet. HasPendingModelChanges must not be used here:
        // it reports model drift from the last snapshot, so an empty database looks up to date.
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            app.Logger.LogInformation("----- {DbName}: creating and migrating the database...", dbName);

            await dbContext.Database.MigrateAsync();

            app.Logger.LogInformation("----- {DbName}: database was created and migrated successfully", dbName);
        }
        else
        {
            app.Logger.LogInformation("----- {DbName}: all migrations are up to date", dbName);
        }
    }

    private static async Task MigrateMongoDbContextAsync(this WebApplication app, IReadDbContext readDbContext)
    {
        app.Logger.LogInformation("----- MongoDB: collections are being created...");

        await readDbContext.CreateCollectionsAsync();

        app.Logger.LogInformation("----- MongoDB: collections were created successfully!");
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Core.SharedKernel;

namespace Shop.Infrastructure.Data.Services;

internal class MemoryCacheService(
    ILogger<MemoryCacheService> logger,
    IMemoryCache memoryCache,
    IOptions<CacheOptions> cacheOptions) : ICacheService
{
    private const string CacheServiceName = nameof(MemoryCacheService);
    private readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheOptions.Value.AbsoluteExpirationInHours),
        SlidingExpiration = TimeSpan.FromSeconds(cacheOptions.Value.SlidingExpirationInSeconds)
    };

    public async Task<TItem> GetOrCreateAsync<TItem>(string cacheKey, Func<Task<TItem>> factory)
    {
        // GetOrCreateAsync must not be used here: it commits the factory result to an entry that
        // carries no expiration, which both overrides these options and caches absent values.
        if (memoryCache.TryGetValue(cacheKey, out TItem cachedItem))
        {
            logger.LogInformation("----- Fetched from {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
            return cachedItem;
        }

        var item = await factory();
        if (!item.IsDefault()) // SonarQube Bug: item != nulll
        {
            logger.LogInformation("----- Added to {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
            memoryCache.Set(cacheKey, item, _cacheOptions);
        }

        return item;
    }

    public async Task<IReadOnlyList<TItem>> GetOrCreateAsync<TItem>(
        string cacheKey,
        Func<Task<IReadOnlyList<TItem>>> factory)
    {
        if (memoryCache.TryGetValue(cacheKey, out IReadOnlyList<TItem> cachedItems))
        {
            logger.LogInformation("----- Fetched from {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
            return cachedItems;
        }

        var items = await factory();
        if (items?.Any() == true)
        {
            logger.LogInformation("----- Added to {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
            memoryCache.Set(cacheKey, items, _cacheOptions);
        }

        return items;
    }

    public Task RemoveAsync(params string[] cacheKeys)
    {
        foreach (var cacheKey in cacheKeys)
        {
            logger.LogInformation("----- Removed from {CacheServiceName}: '{CacheKey}'", CacheServiceName, cacheKey);
            memoryCache.Remove(cacheKey);
        }

        return Task.CompletedTask;
    }
}
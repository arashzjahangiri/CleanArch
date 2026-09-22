using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Infrastructure.Data.Services;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Infrastructure.Data.Services;

[UnitTest]
public class MemoryCacheServiceTests
{
    private const int AbsoluteExpirationInHours = 2;
    private const int SlidingExpirationInSeconds = 60;
    private const string CacheKey = "customers";

    [Fact]
    public async Task Should_InvokeFactoryOnce_When_ItemIsAlreadyCached()
    {
        // Arrange
        var service = CreateService(new MemoryCache(new MemoryCacheOptions()));
        var factoryCalls = 0;

        // Act
        var first = await service.GetOrCreateAsync(CacheKey, _ => { factoryCalls++; return Task.FromResult("value"); });
        var second = await service.GetOrCreateAsync(CacheKey, _ => { factoryCalls++; return Task.FromResult("value"); });

        // Assert
        factoryCalls.Should().Be(1, "the second call must be served from the cache");
        first.Should().Be("value");
        second.Should().Be("value");
    }

    [Fact]
    public async Task Should_NotCacheAbsentItem_When_FactoryReturnsNull()
    {
        // Arrange
        var service = CreateService(new MemoryCache(new MemoryCacheOptions()));
        var factoryCalls = 0;

        // Act
        var first = await service.GetOrCreateAsync(CacheKey, _ => { factoryCalls++; return Task.FromResult<string>(null); });
        var second = await service.GetOrCreateAsync(CacheKey, _ => { factoryCalls++; return Task.FromResult("found later"); });

        // Assert
        factoryCalls.Should().Be(2, "a missing item must never be cached");
        first.Should().BeNull();
        second.Should().Be("found later");
    }

    [Fact]
    public async Task Should_NotCacheEmptyList_When_FactoryReturnsNoItems()
    {
        // Arrange
        var service = CreateService(new MemoryCache(new MemoryCacheOptions()));
        var factoryCalls = 0;

        // Act
        var first = await service.GetOrCreateAsync<string>(
            CacheKey, _ => { factoryCalls++; return Task.FromResult<IReadOnlyList<string>>([]); });

        var second = await service.GetOrCreateAsync<string>(
            CacheKey, _ => { factoryCalls++; return Task.FromResult<IReadOnlyList<string>>(["one"]); });

        // Assert
        factoryCalls.Should().Be(2, "an empty result must never be cached");
        first.Should().BeEmpty();
        second.Should().ContainSingle().Which.Should().Be("one");
    }

    [Fact]
    public async Task Should_ApplyConfiguredExpiration_When_ItemIsCached()
    {
        // Arrange
        var cacheEntry = Substitute.For<ICacheEntry>();
        var memoryCache = Substitute.For<IMemoryCache>();
        memoryCache.CreateEntry(CacheKey).Returns(cacheEntry);

        var service = CreateService(memoryCache);

        // Act
        await service.GetOrCreateAsync(CacheKey, _ => Task.FromResult("value"));

        // Assert
        memoryCache.Received(1).CreateEntry(CacheKey);
        cacheEntry.Received().AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(AbsoluteExpirationInHours);
        cacheEntry.Received().SlidingExpiration = TimeSpan.FromSeconds(SlidingExpirationInSeconds);
    }

    [Fact]
    public async Task Should_InvokeFactoryAgain_When_KeyIsRemoved()
    {
        // Arrange
        var service = CreateService(new MemoryCache(new MemoryCacheOptions()));
        var factoryCalls = 0;

        // Act
        await service.GetOrCreateAsync(CacheKey, _ => { factoryCalls++; return Task.FromResult("value"); });
        await service.RemoveAsync([CacheKey]);
        await service.GetOrCreateAsync(CacheKey, _ => { factoryCalls++; return Task.FromResult("value"); });

        // Assert
        factoryCalls.Should().Be(2, "removal must evict the entry");
    }

    private static MemoryCacheService CreateService(IMemoryCache memoryCache) =>
        new(NullLogger<MemoryCacheService>.Instance, memoryCache, Options.Create(BuildCacheOptions()));

    private static CacheOptions BuildCacheOptions() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "CacheOptions:AbsoluteExpirationInHours", AbsoluteExpirationInHours.ToString() },
                { "CacheOptions:SlidingExpirationInSeconds", SlidingExpirationInSeconds.ToString() }
            })
            .Build()
            .GetOptions<CacheOptions>();
}

using System;
using System.Collections.Generic;

namespace Shop.Query.QueriesModel;

/// <summary>
/// One page of query models plus the totals a client needs to request the next one.
/// </summary>
/// <typeparam name="TQueryModel">The type of the query model.</typeparam>
public sealed class PagedQueryResult<TQueryModel>(
    IReadOnlyList<TQueryModel> items,
    int pageNumber,
    int pageSize,
    long totalCount)
{
    public IReadOnlyList<TQueryModel> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public long TotalCount { get; } = totalCount;

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

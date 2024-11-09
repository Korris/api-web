using Microsoft.EntityFrameworkCore;

namespace Mcsg.Common.Models;

using SeedWork.Responses;

public class PaginatedList<T> : PagedResponse<T>
{
    public PaginatedList(int totalItems, int pageNumber = 1, int pageSize = 10) : base(totalItems, pageNumber, pageSize)
    {
    }

    public PaginatedList(List<T> items, int totalItems, int pageNumber = 1, int pageSize = 10) : base(items, totalItems, pageNumber, pageSize)
    {
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, IQueryable<T> count, int pageNumber, int pageSize)
    {
        var totalItem = await count.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, totalItem, pageNumber, pageSize);
    }
    public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, IQueryable<T> count, int pageNumber, int pageSize)
    {
        var totalItem = await count.CountAsync();
        var items = source.ToList();

        return new PaginatedList<T>(items, totalItem, pageNumber, pageSize);
    }
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
    public static PaginatedList<T> Create(IEnumerable<T> source, IEnumerable<T> count, int pageNumber, int pageSize)
    {
        var totalItem = count.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<T>(items, totalItem, pageNumber, pageSize);
    }
}

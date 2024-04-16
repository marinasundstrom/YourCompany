namespace YourBrand.Application.Common.Models;

public record class ItemResult<T>(IEnumerable<T> Items, int TotalCount);
namespace FishMarket.Api.Dtos;

/// <summary>
/// Represents a paginated collection of items.
/// </summary>
/// <typeparam name="TEntity">The type of the items in the collection.</typeparam>
/// <param name="index">The index of the page.</param>
/// <param name="size">The size of the page.</param>
/// <param name="count">The total number of items in the collection.</param>
/// <param name="data">The items in the collection.</param>
public class PaginatedItems<TEntity>(int index, int size, long count, IReadOnlyCollection<TEntity> data)
    where TEntity : class
{
    /// <summary>
    /// Gets the index of the page.
    /// </summary>
    public int Index { get; } = index;

    /// <summary>
    /// Gets the size of the page.
    /// </summary>
    public int Size { get; } = size;

    /// <summary>
    /// Gets the total number of items in the collection.
    /// </summary>
    public long Count { get; } = count;

    /// <summary>
    /// Gets the items in the collection.
    /// </summary>
    public IReadOnlyCollection<TEntity> Data { get;} = data;
}
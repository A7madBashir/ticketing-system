using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.DTO.Responses;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Handlers.DataTable;

public class DataTableHandler<TEntity, T>(IRepository<TEntity, T> repository, Mapper mapper)
    where TEntity : class, IEntity<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Retrieves paginated, filtered, sorted data with navigation properties and a custom selector.
    /// </summary>
    /// <typeparam name="TEntity">The database entity type.</typeparam>
    /// <typeparam name="TResponse">The type of the result objects (can be an entity, DTO, or anonymous type).</typeparam>
    /// <param name="page">The starting offset for pagination (0-based).</param>
    /// <param name="length">The number of items to retrieve (page size).</param>
    /// <param name="searchTerm">A general search term to apply to string properties.</param>
    /// <param name="searchableProperties">Names of string properties in TEntity to search within.</param>
    /// <param name="whereStatement">An optional LINQ Expression for initial filtering.</param>
    /// <param name="orderBy">An optional LINQ Expression for primary sorting. If null, sorts by Id.</param>
    /// <param name="orderByDescending">If true, sorts by orderBy expression in descending order.</param>
    /// <param name="includeNavigations">An array of navigation property names to include (e.g., "User", "Category").</param>
    /// <param name="selector">A LINQ Expression to select/project the final TResult type.</param>
    /// <returns>A tuple containing total filtered count and the paginated, selected data.</returns>
    public async Task<PaginatedResponse<TResponse>> PaginatedDataAsync<TResponse>(
        int page,
        int length,
        int draw,
        IQueryable<TEntity>? q,
        string? searchTerm,
        string[] searchableProperties,
        Expression<Func<TEntity, bool>>? whereStatement,
        Expression<Func<TEntity, object>>? orderBy,
        bool orderByDescending,
        string[]? includeNavigations,
        Func<TEntity, TResponse> selector
    )
        where TResponse : BaseResponse // The result can be a DTO, entity, or anonymous type
    {
        if (page < 1)
        {
            throw new Exception("Page number must be greater than zero");
        }

        if (length < -1)
        {
            throw new Exception("Length number must be greater than zero");
        }

        IQueryable<TEntity> query;

        if (q is not null)
            query = q;
        else
            query = repository.Query(includeNavigations ?? []);

        int totalRecords = await query.CountAsync();
        int filteredRecords = totalRecords;

        if (whereStatement != null)
        {
            query = query.Where(whereStatement);
        }

        if (
            !string.IsNullOrWhiteSpace(searchTerm)
            && searchableProperties != null
            && searchableProperties.Any()
        )
        {
            query = ApplySearch(query, searchTerm, searchableProperties);
        }

        if (orderBy != null)
        {
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        else // Default sort by Id if no explicit orderBy is provided
        {
            // Ensure TEntity has an 'Id' property that can be ordered.
            // This assumes your IEntity<Ulid> ensures an Id property exists.
            query = query.OrderBy(e => e.Id);
        }

        var paginatedQuery = query;

        if (length > -1) // -1 refer to all result
        {
            paginatedQuery = query.Skip((page - 1) * length).Take(length);
        }

        var data = await paginatedQuery
        .Select((r) => selector(r)) // Apply the projection (mapping)
        .ToListAsync();

        return new PaginatedResponse<TResponse>
        {
            Data = data,
            Draw = draw,
            RecordsFiltered = filteredRecords,
            RecordsTotal = totalRecords,
            HasMoreData = ((page - 1) * length + length) < filteredRecords,
        };
    }

    // --- Helper Methods for Dynamic Querying (Copied from DynamicQueryService for self-containment) ---

    private IQueryable<T> ApplySearch<T>(
        IQueryable<T> query,
        string searchTerm,
        params string[] searchProperties
    )
    {
        if (
            string.IsNullOrWhiteSpace(searchTerm)
            || searchProperties == null
            || searchProperties.Length == 0
        )
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? finalOrExpression = null;
        var lowerSearchTerm = searchTerm.ToLower();

        foreach (var propName in searchProperties)
        {
            var prop = typeof(T).GetProperty(
                propName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );
            if (prop != null && prop.PropertyType == typeof(string))
            {
                var propertyAccess = Expression.Property(parameter, prop);
                var toLowerMethod = typeof(string).GetMethod(
                    nameof(string.ToLower),
                    Type.EmptyTypes
                );
                var containsMethod = typeof(string).GetMethod(
                    nameof(string.Contains),
                    new[] { typeof(string) }
                );

                if (toLowerMethod == null || containsMethod == null)
                    continue;

                var lowercasedProperty = Expression.Call(propertyAccess, toLowerMethod);
                var constantSearchTerm = Expression.Constant(lowerSearchTerm);
                var containsExpression = Expression.Call(
                    lowercasedProperty,
                    containsMethod,
                    constantSearchTerm
                );

                finalOrExpression =
                    finalOrExpression == null
                        ? containsExpression
                        : Expression.OrElse(finalOrExpression, containsExpression);
            }
        }

        if (finalOrExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(finalOrExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    // Note: Filters (Dictionary) are not included in this request,
    // but can be re-added from the previous DynamicQueryService if needed.
    // private IQueryable<T> ApplyFilters<T>(IQueryable<T> query, Dictionary<string, string> filters) { ... }
}

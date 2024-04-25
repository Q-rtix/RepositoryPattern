using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("RepositoryPattern.EntityFrameworkCore.Tests")]
namespace RepositoryPattern.EntityFrameworkCore.Repositories;

internal static class RepositoryQueryExtensions
{
	internal static IQueryable<T> WithTrackingOption<T>(this IQueryable<T> query, bool disableTracking = false)
		where T : class
	{
		ArgumentNullException.ThrowIfNull(query);
		
		return disableTracking ? query.AsNoTracking() : query;
	}

	internal static IQueryable<T> WithIncludedProperties<T>(this IQueryable<T> query,
		IEnumerable<Expression<Func<T, object>>>? includes = null) where T : class
	{
		ArgumentNullException.ThrowIfNull(query);
		
		return includes?.Aggregate(query, (current, includeProperty) => current.Include(includeProperty))
		       ?? query;
	}

	internal static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query,
		Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
	{
		ArgumentNullException.ThrowIfNull(query);
		
		return orderBy is null ? query : orderBy(query);
	}
	
	internal static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query,
		Expression<Func<T, bool>>? filters = null)
	{
		ArgumentNullException.ThrowIfNull(query);
		
		return filters is null ? query : query.Where(filters);
	}
}
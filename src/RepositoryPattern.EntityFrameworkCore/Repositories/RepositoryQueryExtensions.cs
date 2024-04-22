using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("RepositoryPattern.EntityFameworkCore.Tests")]
namespace RepositoryPattern.EntityFrameworkCore.Repositories;

internal static class RepositoryQueryExtensions
{
	internal static IQueryable<T> WithTrackingOption<T>(this IQueryable<T> query, bool disableTracking = false)
		where T : class
		=> disableTracking ? query.AsNoTracking() : query;
	
	internal static IQueryable<T> WithIncludedProperties<T>(this IQueryable<T> query,
		IEnumerable<Expression<Func<T, object>>>? includes) where T : class
		=> includes?.Aggregate(query, (current, includeProperty) => current.Include(includeProperty))
		   ?? query;

	internal static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query,
		Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy)
		=> orderBy is null
			? query
			: orderBy(query);
}
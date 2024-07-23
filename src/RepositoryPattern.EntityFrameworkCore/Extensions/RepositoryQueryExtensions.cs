using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("RepositoryPattern.EntityFrameworkCore.Tests")]
namespace RepositoryPattern.EntityFrameworkCore.Extensions;

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

		if (includes is null)
			return query;
		
		return includes.Aggregate(query, (current, includeExpression) =>
		{
			var path = includeExpression.ToString();
			var split = path.Split(".").AsEnumerable();
			split = split.Where(s => !s.Contains("=>") && !s.Contains("Then("))
				.Select(s => s.Trim('(', ')'));
			path = string.Join(".", split);
			return current.Include(path);
		});
	}

	internal static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query,
		Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
	{
		ArgumentNullException.ThrowIfNull(query);
		
		return orderBy?.Invoke(query) ?? query;
	}
	
	internal static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query,
		Expression<Func<T, bool>>[]? filters = null)
	{
		ArgumentNullException.ThrowIfNull(query);

		return filters?.Aggregate(query, (current, filter) => current.Where(filter))
			?? query;
	}
}
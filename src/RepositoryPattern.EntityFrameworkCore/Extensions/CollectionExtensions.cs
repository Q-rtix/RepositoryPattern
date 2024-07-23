using System.Linq.Expressions;

namespace RepositoryPattern.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension class for collections.
/// </summary>
public static class CollectionExtensions
{
	/// <summary>
	/// Provides access to the <paramref name="navigationPropertyPath"/> to facilitate navigation
	/// among the properties of the object within the collection. The returned object is solely for
	/// compliance with a requirement where this method will be called, and no operations should be
	/// performed on this returned object.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities in the collection.</typeparam>
	/// <typeparam name="TProperty">The type of the property to navigate to.</typeparam>
	/// <param name="src">The source collection.</param>
	/// <param name="navigationPropertyPath">An expression indicating the property path for navigation.</param>
	/// <returns>An object for compliance with method calling requirements; not intended for further operations.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="src"/> or <paramref name="navigationPropertyPath"/> is null.</exception>
	public static object Then<TEntity, TProperty>(this ICollection<TEntity> src,
		Expression<Func<TEntity, TProperty>> navigationPropertyPath)
	{
		ArgumentNullException.ThrowIfNull(src);
		ArgumentNullException.ThrowIfNull(navigationPropertyPath);

		return  navigationPropertyPath;
	}
}
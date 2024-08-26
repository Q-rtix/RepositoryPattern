using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Abstractions.Extensions;
using RepositoryPattern.Abstractions.Repositories;

[assembly: InternalsVisibleTo("RepositoryPattern.Abstractions.Tests")]

namespace RepositoryPattern.Abstractions.Builder;

/// <summary>
///     Represents the options for configuring the implementation types for the repository pattern.
/// </summary>
public class RepositoryOptions
{
	/// <summary>
	///     Configures the repository implementation type to be used.
	/// </summary>
	/// <param name="implementationType">
	///		The type to be used for the repository implementation.
	/// </param>
	/// <exception cref="ArgumentException">
	///		Thrown when the provided type does not implement the <see cref="IRepository{TEntity}" /> interface.
	/// </exception>
	public RepositoryOptions(Type implementationType)
	{
		if (!implementationType.IsImplementingRepository(typeof(IRepository<>)))
			throw new ArgumentException("The provided type must be class that implement the IRepository<TEntity> interface");
		ImplementationType = implementationType;
	}

	/// <summary>
	///		Configures the lifetime of the service.
	/// </summary>
	/// <param name="lifetime">The lifetime of the service.</param>
	/// <returns>The current instance of the <see cref="RepositoryOptions" />.</returns>
	public RepositoryOptions UseLifetime(ServiceLifetime lifetime)
	{
		Lifetime = lifetime;
		return this;
	}

	internal Type ImplementationType { get; private set; }
	internal ServiceLifetime Lifetime { get; private set; } = ServiceLifetime.Scoped;
}
using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Abstractions.Extensions;
using RepositoryPattern.Abstractions.UnitOfWork;

namespace RepositoryPattern.Abstractions.Builder;

/// <summary>
///     Represents the options for configuring the implementation types for the unit of work.
/// </summary>
public class UnitOfWorkOptions
{
	/// <summary>
	///     Configures the unit of work implementation type to be used.
	/// </summary>
	/// <param name="implementationType">
	///     The type to be used for the unit of work implementation.
	/// </param>
	/// <exception cref="ArgumentException">
	///     Thrown when the provided type does not implement the <see cref="IUnitOfWork" /> interface.
	/// </exception>
	public UnitOfWorkOptions(Type implementationType)
	{
		if (!implementationType.IsImplementing(typeof(IUnitOfWork)))
			throw new ArgumentException("The provided type must be class that implement the IUnitOfWork interface");
		ImplementationType = implementationType;
	}

	/// <summary>
	///     Configures the lifetime of the service.
	/// </summary>
	/// <param name="lifetime">The lifetime of the service.</param>
	/// <returns>The current instance of the <see cref="UnitOfWorkOptions" />.</returns>
	public UnitOfWorkOptions UseLifetime(ServiceLifetime lifetime)
	{
		Lifetime = lifetime;
		return this;
	}
	
	internal Type ImplementationType { get; private set; }
	internal ServiceLifetime Lifetime { get; private set; } = ServiceLifetime.Scoped;
}
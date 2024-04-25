using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RepositoryPattern.Abstractions.Builder;
using RepositoryPattern.Abstractions.Repositories;
using RepositoryPattern.Abstractions.UnitOfWork;

namespace RepositoryPattern.Abstractions;

/// <summary>
///     Contain the methods to integrate this library.
/// </summary>
public static class DependencyInjection
{
	/// <summary>
	///     Integrate and configure the requisite dependencies and services for seamless operation of this library.
	/// </summary>
	/// <param name="services">An <see cref="IServiceCollection" /> to add the requisite dependencies and services.</param>
	/// <param name="options"></param>
	/// <param name="servicesLifeTime">
	///     The lifetime with wich to register the repository and unit of work services in the
	///     container.
	/// </param>
	/// <returns>
	///     An <see cref="IServiceCollection" /> with the requisite dependencies and services for seamless operation of
	///     this library.
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when the <see cref="IServiceCollection" /> is null</exception>
	/// <exception cref="InvalidOperationException">Thrown when the repository pattern options are not configured</exception>
	public static IServiceCollection AddRepositoryPattern(this IServiceCollection services,
		Action<RepositoryPatternOptions> options, ServiceLifetime servicesLifeTime = ServiceLifetime.Scoped)
	{
		ArgumentNullException.ThrowIfNull(services);

		var repositoryPatternOptions = new RepositoryPatternOptions();
		options(repositoryPatternOptions);

		services.TryAdd(new ServiceDescriptor(typeof(IRepository<>), repositoryPatternOptions.RepositoryImplementationType,
			servicesLifeTime));
		services.TryAdd(new ServiceDescriptor(typeof(IUnitOfWork), repositoryPatternOptions.UnitOfWorkImplementation, servicesLifeTime));

		return services;
	}
}
using Microsoft.Extensions.DependencyInjection;
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
	/// <exception cref="ArgumentNullException">If the <see cref="IServiceCollection" /> is null</exception>
	public static IServiceCollection AddRepositoryPattern(this IServiceCollection services,
		Action<RepositoryPatternOptionBuilder> options, ServiceLifetime servicesLifeTime = ServiceLifetime.Scoped)
	{
		ArgumentNullException.ThrowIfNull(services);

		var builder = new RepositoryPatternOptionBuilder();
		options(builder);

		services.Add(new ServiceDescriptor(typeof(IRepository<>), builder.RepositoryImplementationType,
			servicesLifeTime));
		services.Add(new ServiceDescriptor(typeof(IUnitOfWork), builder.UnitOfWorkImplementation, servicesLifeTime));

		return services;
	}
}
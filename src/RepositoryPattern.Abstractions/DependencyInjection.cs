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
	/// <returns>
	///     An <see cref="IServiceCollection" /> with the requisite dependencies and services for seamless operation of
	///     this library.
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when the <see cref="IServiceCollection" /> is null</exception>
	/// <exception cref="InvalidOperationException">Thrown when the repository pattern options are not configured or has invalid values</exception>
	public static IServiceCollection AddRepositoryPattern(this IServiceCollection services,
		Action<RepositoryPatternOptions> options)
	{
		ArgumentNullException.ThrowIfNull(services);

		var repositoryPatternOptions = new RepositoryPatternOptions();
		options(repositoryPatternOptions);
		
		repositoryPatternOptions.IsValid();

		services.TryAdd(new ServiceDescriptor(typeof(IRepository<>),
			repositoryPatternOptions.RepositoryImplementationType!,
			repositoryPatternOptions.RepositoryServiceLifetime));

		services.TryAdd(new ServiceDescriptor(typeof(IUnitOfWork), repositoryPatternOptions.UnitOfWorkImplementation!,
			repositoryPatternOptions.UnitOfWorkServiceLifetime));

		return services;
	}
}
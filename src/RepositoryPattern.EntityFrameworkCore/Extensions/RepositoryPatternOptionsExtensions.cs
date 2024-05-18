using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Abstractions.Builder;
using RepositoryPattern.EntityFrameworkCore.Repositories;
using RepositoryPattern.EntityFrameworkCore.UnitsOfWork;

namespace RepositoryPattern.EntityFrameworkCore.Extensions;

/// <summary>
///		Extension methods for setting up the repository pattern related services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
/// </summary>
public static class RepositoryPatternOptionsExtensions
{
	/// <summary>
	///		Configures the repository pattern to use Entity Framework Core as the implementation of the repository and unit of work.
	/// </summary>
	/// <param name="builder">The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance to configure.</param>
	/// <returns>The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance so that additional calls can be chained.</returns>
	public static RepositoryPatternBuilder UseEntityFrameworkCore(this RepositoryPatternBuilder builder)
	{
		builder.UseRepositoryImplementation(typeof(Repository<>));
		builder.UseUnitOfWorkImplementation<UnitOfWork>();

		return builder;
	}
	
	/// <summary>
	/// 	Configures the repository pattern to use Entity Framework Core as the implementation of the repository and unit of work.
	/// </summary>
	/// <param name="builder">The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance to configure.</param>
	/// <param name="repositoryServiceLifetime">
	///		The lifetime with which to register the repository service in the service collection.
	/// </param>
	/// <param name="unitOfWorkServiceLifetime">
	///		The lifetime with which to register the unit of work service in the service collection.
	/// </param>
	/// <returns>The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance so that additional calls can be chained.</returns>
	public static RepositoryPatternBuilder UseEntityFrameworkCore(this RepositoryPatternBuilder builder,
		ServiceLifetime repositoryServiceLifetime, ServiceLifetime unitOfWorkServiceLifetime)
	{
		builder.UseRepositoryImplementation(typeof(Repository<>))
			.UseLifetime(repositoryServiceLifetime);
		
		builder.UseUnitOfWorkImplementation<UnitOfWork>()
			.UseLifetime(unitOfWorkServiceLifetime);

		return builder;
	}

	/// <summary>
	/// 	Configures the repository pattern to use Entity Framework Core as the implementation of the repository and unit of work.
	/// </summary>
	/// <param name="builder">The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance to configure.</param>
	/// <param name="serviceLifetime">
	/// 	The lifetime with which to register the repository and unit of work services in the service collection.
	/// </param>
	/// <returns>The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance so that additional calls can be chained.</returns>
	public static RepositoryPatternBuilder UseEntityFrameworkCore(this RepositoryPatternBuilder builder,
		ServiceLifetime serviceLifetime)
	{
		builder.UseRepositoryImplementation(typeof(Repository<>))
			.UseLifetime(serviceLifetime);
		
		builder.UseUnitOfWorkImplementation<UnitOfWork>()
			.UseLifetime(serviceLifetime);
		
		return builder;
	}
}
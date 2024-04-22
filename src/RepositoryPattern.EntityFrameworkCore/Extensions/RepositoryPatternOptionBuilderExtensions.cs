using RepositoryPattern.Abstractions.Builder;
using RepositoryPattern.EntityFrameworkCore.Repositories;
using RepositoryPattern.EntityFrameworkCore.UnitsOfWork;

namespace RepositoryPattern.EntityFrameworkCore.Extensions;

/// <summary>
///		Extension methods for setting up the repository pattern related services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
/// </summary>
public static class RepositoryPatternOptionBuilderExtensions
{
	/// <summary>
	///		Configures the repository pattern to use Entity Framework Core as the implementation of the repository and unit of work.
	/// </summary>
	/// <param name="builder">The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance to configure.</param>
	/// <returns>The <see cref="T:RepositoryPattern.Abstractions.Builder.RepositoryPatternOptionBuilder" /> instance so that additional calls can be chained.</returns>
	public static RepositoryPatternOptionBuilder UseEntityFrameworkCore(this RepositoryPatternOptionBuilder builder)
		=> builder.UseRepositoryImplementation(typeof(Repository<>))
			.UseUnitOfWorkImplementation<UnitOfWork>();
}
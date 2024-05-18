using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Abstractions.Repositories;
using RepositoryPattern.Abstractions.UnitOfWork;

namespace RepositoryPattern.Abstractions.Builder;

/// <summary>
///     Represents a builder for configuring the implementation types for the repository pattern.
/// </summary>
/// <remarks>
///     This builder allows configuring the types to be used for repository and unit of work implementations in the
///     repository pattern.
/// </remarks>
public class RepositoryPatternBuilder
{
	
	internal RepositoryOptions? Repository { get; private set; }

	internal UnitOfWorkOptions? UnitOfWork { get; private set; }

	/// <summary>
	///		Configures the repository implementation type to be used.
	/// </summary>
	/// <param name="repositoryImplementationType">The type to be used for the repository implementation.</param>
	/// <returns>The current instance of the <see cref="RepositoryPatternBuilder" />.</returns>
	/// <exception cref="ArgumentException">
	///     Thrown when the provided type does not implement the
	///     <see cref="IRepository{TEntity}" /> interface.
	/// </exception>
	public RepositoryOptions UseRepositoryImplementation(Type repositoryImplementationType)
	{
		Repository = new RepositoryOptions(repositoryImplementationType);
		return Repository;
	}

	/// <summary>
	///		Configures the Unit of work implementation type to be used.
	/// </summary>
	/// <param name="unitOfWorkImplementationType">The type to be used for the unit of work implementation.</param>
	/// <returns>The current instance of the <see cref="RepositoryPatternBuilder" />.</returns>
	/// <exception cref="ArgumentException">
	///     Thrown when the provided type does not implement the <see cref="IUnitOfWork" />
	///     interface.
	/// </exception>
	public UnitOfWorkOptions UseUnitOfWorkImplementation(Type unitOfWorkImplementationType)
	{
		UnitOfWork = new UnitOfWorkOptions(unitOfWorkImplementationType);
		return UnitOfWork;
	}

	/// <summary>
	///     Configures the repository implementation type to be used.
	/// </summary>
	/// <typeparam name="TUnitOfWork">The type to be used for the unit of work implementation.</typeparam>
	/// <returns>The current instance of the <see cref="RepositoryPatternBuilder" />.</returns>
	public UnitOfWorkOptions UseUnitOfWorkImplementation<TUnitOfWork>()
		where TUnitOfWork : class, IUnitOfWork
	{
		UnitOfWork = new UnitOfWorkOptions(typeof(TUnitOfWork));
		return UnitOfWork;
	}
	
	internal void IsValid()
	{
		if (Repository is null)
			throw new InvalidOperationException("The repository options has not been configured");
		
		if (UnitOfWork is null)
			throw new InvalidOperationException("The unit of work options has not been configured");
	}
}
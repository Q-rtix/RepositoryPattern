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
public class RepositoryPatternOptions
{
	
	/// <summary>
	///		The type to be used for the repository implementation.
	/// </summary>
	public Type RepositoryImplementationType { get; private set; } = null!;

	/// <summary>
	///		The type to be used for the unit of work implementation.
	/// </summary>
	public Type UnitOfWorkImplementation { get; private set; } = null!;

	/// <summary>
	///		Configures the repository implementation type to be used.
	/// </summary>
	/// <param name="repositoryImplementationType">The type to be used for the repository implementation.</param>
	/// <returns>The current instance of the <see cref="RepositoryPatternOptions" />.</returns>
	/// <exception cref="ArgumentException">
	///     Thrown when the provided type does not implement the
	///     <see cref="IRepository{TEntity}" /> interface.
	/// </exception>
	public RepositoryPatternOptions UseRepositoryImplementation(Type repositoryImplementationType)
	{
		if (!IsImplementing(repositoryImplementationType, typeof(IRepository<>)))
			throw new ArgumentException("The provided type must be class that implement the IRepository<TEntity> interface");
		
		RepositoryImplementationType = repositoryImplementationType;
		return this;
	}

	/// <summary>
	///		Configures the Unit of work implementation type to be used.
	/// </summary>
	/// <param name="unitOfWorkImplementationType">The type to be used for the unit of work implementation.</param>
	/// <returns>The current instance of the <see cref="RepositoryPatternOptions" />.</returns>
	/// <exception cref="ArgumentException">
	///     Thrown when the provided type does not implement the <see cref="IUnitOfWork" />
	///     interface.
	/// </exception>
	public RepositoryPatternOptions UseUnitOfWorkImplementation(Type unitOfWorkImplementationType)
	{
		if (!IsImplementing(unitOfWorkImplementationType, typeof(IUnitOfWork)))
			throw new ArgumentException("The provided type must be a class that implements the IUnitOfWork interface");

		UnitOfWorkImplementation = unitOfWorkImplementationType;
		return this;
	}

	/// <summary>
	///     Configures the repository implementation type to be used.
	/// </summary>
	/// <typeparam name="TUnitOfWork">The type to be used for the unit of work implementation.</typeparam>
	/// <returns>The current instance of the <see cref="RepositoryPatternOptions" />.</returns>
	public RepositoryPatternOptions UseUnitOfWorkImplementation<TUnitOfWork>()
		where TUnitOfWork : class, IUnitOfWork
	{
		UnitOfWorkImplementation = typeof(TUnitOfWork);
		return this;
	}

	private static bool IsImplementing(Type type, Type interfaceType)
		=> type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
}
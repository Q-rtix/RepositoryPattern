using RepositoryPattern.Abstractions.Repositories;

namespace RepositoryPattern.Abstractions.UnitOfWork;

/// <summary>
///     Represents a unit of work that coordinates and manages database operations using Entity Framework Core.
///     The `IUnitOfWork` interface serves as a central hub for working with the database context, repositories, and
///     transactions.
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
	/// <summary>
	///     Determines whether the current transaction supports save points.
	/// </summary>
	/// <returns>
	///     <c>true</c> if the transaction supports save points; otherwise, <c>false</c>.
	/// </returns>
	/// <remarks>
	/// This property returns <c>true</c> if the current transaction supports savepoints,
	/// otherwise <c>false</c>.
	/// </remarks>
	bool SupportsSavePoints { get; }

	/// <summary>
	///     Retrieves a repository for managing entities of type <typeparamref name="TEntity" />.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities to be managed by the repository.</typeparam>
	/// <returns>
	///     An instance of <see cref="IRepository{TEntity}" /> for managing entities of the specified type.
	/// </returns>
	IRepository<TEntity> Repository<TEntity>() where TEntity : class;

	/// <summary>
	///     Initiates a new database transaction.
	/// </summary>
	/// <param name="force">
	///     (Optional) Indicates whether to force a new transaction even when another transaction is in progress.
	///     <c>true</c> to force a new transaction; <c>false</c> otherwise. Default is <c>false</c>.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to begin a transaction while another transaction is already in progress.
	/// </exception>
	void BeginTransaction(bool force = false);

	/// <summary>
	///     Asynchronously initiates a new database transaction.
	/// </summary>
	/// <param name="force">
	///     (Optional) Indicates whether to force a new transaction even when another transaction is in progress.
	///     <c>true</c> to force a new transaction; <c>false</c> otherwise. Default is <c>false</c>.
	/// </param>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to begin a transaction while another transaction is already in progress.
	/// </exception>
	Task BeginTransactionAsync(bool force = false, CancellationToken cancellationToken = default);

	/// <summary>
	///     Rolls back the current transaction.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to roll back a transaction that is not in progress.
	/// </exception>
	void RollBack();

	/// <summary>
	///     Asynchronously rolls back the current transaction.
	/// </summary>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to roll back a transaction that is not in progress.
	/// </exception>
	Task RollBackAsync(CancellationToken cancellationToken = default);

	/// <summary>
	///     Rolls back the transaction to a specific savepoint.
	/// </summary>
	/// <param name="name">The name of the savepoint to which the transaction should be rolled back.</param>
	/// <exception cref="InvalidOperationException">
	///     The transaction is not in progress.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to roll back a transaction that is not in progress.
	/// </exception>
	void RollBackToSavePoint(string name);

	/// <summary>
	///     Asynchronously rolls back the transaction to a specific savepoint.
	/// </summary>
	/// <param name="name">The name of the savepoint to which the transaction should be rolled back.</param>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to roll back a transaction that is not in progress.
	/// </exception>
	Task RollBackToSavePointAsync(string name, CancellationToken cancellationToken = default);

	/// <summary>
	///     Commits the current transaction.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to commit a transaction that is not in progress.
	/// </exception>
	void Commit();

	/// <summary>
	///     Asynchronously commits the current transaction.
	/// </summary>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to commit a transaction that is not in progress.
	/// </exception>
	Task CommitAsync(CancellationToken cancellationToken = default);

	/// <summary>
	///     Creates a savepoint within the current transaction.
	/// </summary>
	/// <param name="name">The name of the savepoint to create.</param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to create a save point within a transaction that is not in progress.
	/// </exception>
	void CreateSavePoint(string name);

	/// <summary>
	///     Asynchronously creates a savepoint within the current transaction.
	/// </summary>
	/// <param name="name">The name of the savepoint to create.</param>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to create a save point within a transaction that is not in progress.
	/// </exception>
	Task CreateSavePointAsync(string name, CancellationToken cancellationToken = default);

	/// <summary>
	///     Releases a previously created savepoint within the current transaction.
	/// </summary>
	/// <param name="name">The name of the savepoint to release.</param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to release a save point within a transaction that is not in progress.
	/// </exception>
	void ReleaseSavePoint(string name);

	/// <summary>
	///     Releases a previously created savepoint within the current transaction.
	/// </summary>
	/// <param name="name">The name of the savepoint to release.</param>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to release a save point within a transaction that is not in progress.
	/// </exception>
	Task ReleaseSavePointAsync(string name, CancellationToken cancellationToken = default);

	/// <summary>
	///     Asynchronously saves changes to the database.
	/// </summary>
	/// <param name="cancellationToken">
	///     A <see cref="CancellationToken" /> to observe while waiting for the operation to
	///     complete.
	/// </param>
	/// <returns>
	///     A <see cref="Task" /> representing the asynchronous operation. The task result is the number of entities affected
	///     by the save operation.
	/// </returns>
	Task<int> SaveAsync(CancellationToken cancellationToken = default);

	/// <summary>
	///     Synchronously saves changes to the database.
	/// </summary>
	/// <returns>
	///     The number of entities affected by the save operation.
	/// </returns>
	int Save();
}
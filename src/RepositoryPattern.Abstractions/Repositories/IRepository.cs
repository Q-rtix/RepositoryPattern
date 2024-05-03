using System.Linq.Expressions;

namespace RepositoryPattern.Abstractions.Repositories;

/// <summary>
///     Represents a repository for managing entities of type <typeparamref name="TEntity" />
///     within an Entity Framework Core context.
/// </summary>
/// <typeparam name="TEntity">The type of entities managed by this repository.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
	/// <summary>
	///     Retrieves all entities from the repository.
	/// </summary>
	/// <returns>
	///     An <see cref="IQueryable{TEntity}" /> with all existing <typeparamref name="TEntity" />
	///     elements.
	/// </returns>
	IQueryable<TEntity> Data { get; }

	/// <summary>
	///     Synchronously retrieves multiple entities from the repository based on filters criteria.
	/// </summary>
	/// <param name="filters">
	///     (Optional) A lambda expression to test each entity for a condition. If null, all entities are retrieved.
	/// </param>
	/// <param name="disableTracking">
	///     (Optional) <see langword="true" /> to disable change tracking; otherwise, <see langword="false" />. The default is
	///     <see langword="false" />.
	/// </param>
	/// <param name="orderBy">
	///     (Optional) A lambda expression to specify the order of the retrieved entities.
	/// </param>
	/// <param name="includes">
	///		(Optional) A list with the expressions for related entities to be included with the retrieved entities.
	/// </param>
	/// <returns>
	///     An <see cref="IQueryable{TEntity}" /> with all existing <typeparamref name="TEntity" />
	///     elements if the <paramref name="filters" /> is null. Otherwise, it only contains elements
	///     that satisfy the condition specified by <paramref name="filters" />.
	/// </returns>
	IQueryable<TEntity> GetMany(IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		bool disableTracking = false,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
		params Expression<Func<TEntity, bool>>[]? filters);

	/// <summary>
	///     Asynchronously retrieves a single entity from the repository based on filters criteria.
	/// </summary>
	/// <param name="includes">
	///     (Optional) A list with the expressions for related entities to be included with the retrieved entity.
	/// </param>
	/// <param name="disableTracking">
	///     (Optional) <see langword="true" /> to disable change tracking; otherwise, <see langword="false" />. The default is
	///     <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <param name="filters">
	///     A lambda expression to test each entity for a condition.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result is the retrieved entity, or null if not found.
	/// </returns>
	Task<TEntity?> GetOneAsync(
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		bool disableTracking = false,
		CancellationToken cancellationToken = default,
		params Expression<Func<TEntity, bool>>[] filters
	);

	///  <summary>
	///      Synchronously retrieves a single entity from the repository based on filters criteria.
	///  </summary>
	///  <param name="includes">
	///      (Optional) A list with the expressions for related entities to be included with the retrieved entity.
	///  </param>
	///  <param name="disableTracking">
	///      (Optional) <see langword="true" /> to disable change tracking; otherwise, <see langword="false" />. The default is
	///      <see langword="false" />.
	///  </param>
	///  <param name="filters">
	///      A lambda expressions to test each entity for a condition.
	///  </param>
	///  <returns>
	///      Retrieved the entity that satisfies the condition specified by <paramref name="filters" /> , or null if not found.
	///  </returns>
	TEntity? GetOne(
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		bool disableTracking = false,
		params Expression<Func<TEntity, bool>>[] filters
	);

	/// <summary>
	///     Asynchronously creates a single entity into the repository.
	/// </summary>
	/// <param name="entity">
	///     The entity to be created.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been inserted; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result is the created entity.
	/// </returns>
	Task<TEntity> AddOneAsync(TEntity entity, bool saveChanges = false,
		CancellationToken cancellationToken = default);
	
	/// <summary>
	///     Synchronously creates a single entity into the repository.
	/// </summary>
	/// <param name="entity">
	///     The entity to be created.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been inserted; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <returns>
	///     The created entity.
	/// </returns>
	TEntity AddOne(TEntity entity, bool saveChanges = false);

	/// <summary>
	///     Asynchronously creates multiple entities into the repository.
	/// </summary>
	/// <param name="entities">
	///     A collection of entities to be created.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been inserted; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	/// </returns>
	Task AddManyAsync(IEnumerable<TEntity> entities, bool saveChanges = false,
		CancellationToken cancellationToken = default);
	
	/// <summary>
	///     Synchronously creates multiple entities into the repository.
	/// </summary>
	/// <param name="entities">
	///     A collection of entities to be created.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been inserted; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	void AddMany(IEnumerable<TEntity> entities, bool saveChanges = false);

	/// <summary>
	///     Asynchronously updates a single entity in the repository.
	/// </summary>
	/// <param name="entity">
	///     The entity to be updated.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been updated; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result is the updated entity.
	/// </returns>
	Task<TEntity> UpdateOneAsync(TEntity entity, bool saveChanges = false,
		CancellationToken cancellationToken = default);
	
	/// <summary>
	///     Synchronously updates a single entity in the repository.
	/// </summary>
	/// <param name="entity">
	///     The entity to be updated.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been updated; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <returns>
	///     The updated entity.
	/// </returns>
	TEntity UpdateOne(TEntity entity, bool saveChanges = false);

	/// <summary>
	///     Asynchronously updates multiple entities in the repository.
	/// </summary>
	/// <param name="entities">
	///     A collection of entities to be updated.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been updated; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	/// </returns>
	Task UpdateManyAsync(IEnumerable<TEntity> entities, bool saveChanges = false,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Synchronously updates multiple entities in the repository.
	/// </summary>
	/// <param name="entities">
	///     A collection of entities to be updated.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been updated; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	void UpdateMany(IEnumerable<TEntity> entities, bool saveChanges = false);

	/// <summary>
	///     Asynchronously removes a single entity from the repository.
	/// </summary>
	/// <param name="entity">
	///     The entity to be removed.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result is the removed entity.
	/// </returns>
	Task<TEntity> RemoveOneAsync(TEntity entity, bool saveChanges = false,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Synchronously removes a single entity from the repository.
	/// </summary>
	/// <param name="entity">
	///     The entity to be removed.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <returns>
	///     The removed entity.
	/// </returns>
	TEntity RemoveOne(TEntity entity, bool saveChanges = false);

	/// <summary>
	///     Asynchronously removes a single entity from the repository based on filters criteria.
	/// </summary>
	/// <param name="filters">
	///     A lambda expression to test each entity for a condition.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result is the removed entity.
	/// </returns>
	/// <exception cref="System.ArgumentException">
	///     Thrown when no entity matching the filters criteria is found in the repository.
	/// </exception>
	Task<TEntity> RemoveOneAsync(Expression<Func<TEntity, bool>>[] filters, bool saveChanges = false,
		CancellationToken cancellationToken = default);
	
	/// <summary>
	///     Synchronously removes a single entity from the repository based on filters criteria.
	/// </summary>
	/// <param name="filters">
	///     A lambda expression to test each entity for a condition.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entity has been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <returns>
	///     The removed entity.
	/// </returns>
	/// <exception cref="System.ArgumentException">
	///     Thrown when no entity matching the filters criteria is found in the repository.
	/// </exception>
	TEntity RemoveOne(Expression<Func<TEntity, bool>>[] filters, bool saveChanges = false);

	/// <summary>
	///     Asynchronously removes multiple entities from the repository.
	/// </summary>
	/// <param name="entities">
	///     A collection of entities to be removed.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	/// </returns>
	Task RemoveManyAsync(IEnumerable<TEntity> entities, bool saveChanges = false,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Synchronously removes multiple entities from the repository.
	/// </summary>
	/// <param name="entities">
	///     A collection of entities to be removed.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	void RemoveMany(IEnumerable<TEntity> entities, bool saveChanges = false);

	/// <summary>
	///     Asynchronously removes multiple entities from the repository based on filters criteria.
	/// </summary>
	/// <param name="filters">
	///     A lambda expression to test each entity for a condition.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	/// </returns>
	Task RemoveManyAsync(Expression<Func<TEntity, bool>>[] filters, bool saveChanges = false,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Synchronously removes multiple entities from the repository based on filters criteria.
	/// </summary>
	/// <param name="filters">
	///     A lambda expression to test each entity for a condition.
	/// </param>
	/// <param name="saveChanges">
	///     (Optional) <see langword="true" /> to save changes after the entities have been removed; otherwise,
	///     <see langword="false" />. The default is <see langword="false" />.
	/// </param>
	void RemoveMany(Expression<Func<TEntity, bool>>[] filters, bool saveChanges = false);

	/// <summary>
	///     Asynchronously saves changes to the repository.
	/// </summary>
	/// <param name="cancellationToken">
	///     (Optional) A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
	/// </param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	///     The task result is the number of entities affected by the save operation.
	/// </returns>
	Task<int> SaveAsync(CancellationToken cancellationToken = default);
	
	/// <summary>
	///     Synchronously saves changes to the repository.
	/// </summary>
	/// <returns>
	///     The number of entities affected by the save operation.
	/// </returns>
	int Save();
}
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Abstractions.Repositories;
using RepositoryPattern.EntityFrameworkCore.Extensions;

[assembly: InternalsVisibleTo("RepositoryPattern.EntityFrameworkCore.Tests")]

namespace RepositoryPattern.EntityFrameworkCore.Repositories;

internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
	private readonly DbSet<TEntity> _dbSet;

	public Repository(DbContext context)
	{
		_dbSet = context.Set<TEntity>();
	}


	public IQueryable<TEntity> GetMany(IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		bool disableTracking = false, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
		params Expression<Func<TEntity, bool>>[]? filters)
		=> _dbSet.WithTrackingOption(disableTracking)
			.WithIncludedProperties(includes)
			.ApplyFiltering(filters)
			.ApplyOrdering(orderBy);

	public async Task<TEntity?> GetOneAsync(IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		bool disableTracking = false,
		CancellationToken cancellationToken = default,
		params Expression<Func<TEntity, bool>>[] filters)
		=> await _dbSet.WithTrackingOption(disableTracking)
			.WithIncludedProperties(includes)
			.ApplyFiltering(filters)
			.FirstOrDefaultAsync(cancellationToken);


	public TEntity? GetOne(IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		bool disableTracking = false, params Expression<Func<TEntity, bool>>[] filters)
		=> _dbSet.WithTrackingOption(disableTracking)
			.WithIncludedProperties(includes)
			.ApplyFiltering(filters)
			.FirstOrDefault();


	public Task AddOneAsync(TEntity entity,
		CancellationToken cancellationToken = default)
		=> _dbSet.AddAsync(entity, cancellationToken).AsTask();

	public void AddOne(TEntity entity) => _dbSet.Add(entity);

	public Task AddManyAsync(IEnumerable<TEntity> entities,
		CancellationToken cancellationToken = default)
		=> _dbSet.AddRangeAsync(entities, cancellationToken);

	public void AddMany(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

	public void UpdateOne(TEntity entity) => _dbSet.Update(entity);

	public void UpdateMany(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

	public void RemoveOne(TEntity entity) => _dbSet.Remove(entity);

	public void RemoveOne(Expression<Func<TEntity, bool>>[] filters)
	{
		var entity = GetOne(filters: filters);
		if (entity is null)
			throw new ArgumentException($"{typeof(TEntity).Name} not found", nameof(filters));

		RemoveOne(entity);
	}

	public void RemoveMany(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

	public void RemoveMany(Expression<Func<TEntity, bool>>[] filters)
	{
		var entities = GetMany(filters: filters);
		if (!entities.Any())
			return;
		RemoveMany(entities);
	}

	public IEnumerator<TEntity> GetEnumerator()
		=> _dbSet.AsQueryable().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	public Type ElementType => _dbSet.AsQueryable().ElementType;
	public Expression Expression => _dbSet.AsQueryable().Expression;
	public IQueryProvider Provider => _dbSet.AsQueryable().Provider;
}
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Abstractions.Repositories;

[assembly: InternalsVisibleTo("RepositoryPattern.EntityFrameworkCore.Tests")]
namespace RepositoryPattern.EntityFrameworkCore.Repositories;

internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
	private readonly DbSet<TEntity> _dbSet;
	private readonly DbContext _context;

	
	public Repository(DbContext context)
	{
		_context = context;
		_dbSet = context.Set<TEntity>();
	}

	
	public IQueryable<TEntity> Data => _dbSet;


	public async Task<IQueryable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>>? filters = null,
		bool disableTracking = true,
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
		CancellationToken cancellationToken = default)
		=> await Task.Run(() =>
				_dbSet.WithTrackingOption(disableTracking).WithIncludedProperties(includes).ApplyOrdering(orderBy)
			, cancellationToken);

	public IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>>? filters = null,
		bool disableTracking = false,
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
		=> _dbSet.WithTrackingOption(disableTracking)
			.WithIncludedProperties(includes)
			.ApplyOrdering(orderBy);

	public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filters,
		bool disableTracking = false,
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		CancellationToken cancellationToken = default)
		=> await _dbSet.WithTrackingOption(disableTracking)
			.WithIncludedProperties(includes)
			.FirstOrDefaultAsync(filters, cancellationToken);


	public TEntity? GetOne(Expression<Func<TEntity, bool>> filters, bool disableTracking = false,
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null)
		=> _dbSet.WithTrackingOption(disableTracking)
			.WithIncludedProperties(includes)
			.FirstOrDefault(filters);


	public async Task<TEntity> AddOneAsync(TEntity entity, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		var entry = await _dbSet.AddAsync(entity, cancellationToken);
		if (saveChanges)
			await _context.SaveChangesAsync(cancellationToken);
		return entry.Entity;
	}

	public TEntity AddOne(TEntity entity, bool saveChanges = false)
	{
		var entry = _dbSet.Add(entity);
		if (saveChanges)
			_context.SaveChanges();
		return entry.Entity;
	}


	public async Task AddManyAsync(IEnumerable<TEntity> entities, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		await _dbSet.AddRangeAsync(entities, cancellationToken);
		if (saveChanges)
			await _context.SaveChangesAsync(cancellationToken);
	}

	public void AddMany(IEnumerable<TEntity> entities, bool saveChanges = false)
	{
		_dbSet.AddRange(entities);
		if (saveChanges)
			_context.SaveChanges();
	}

	public async Task<TEntity> UpdateOneAsync(TEntity entity, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		var entry = _dbSet.Update(entity);
		if (saveChanges)
			await _context.SaveChangesAsync(cancellationToken);
		return entry.Entity;
	}

	public TEntity UpdateOne(TEntity entity, bool saveChanges = false)
	{
		var entry = _dbSet.Update(entity);
		if (saveChanges)
			_context.SaveChanges();
		return entry.Entity;
	}

	public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		_dbSet.UpdateRange(entities);
		if (saveChanges)
			await _context.SaveChangesAsync(cancellationToken);
	}

	public void UpdateMany(IEnumerable<TEntity> entities, bool saveChanges = false)
	{
		_dbSet.UpdateRange(entities);
		if (saveChanges)
			_context.SaveChanges();
	}


	public async Task<TEntity> RemoveOneAsync(TEntity entity, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		var entry = _dbSet.Remove(entity);
		if (saveChanges)
			await _context.SaveChangesAsync(cancellationToken);
		return entry.Entity;
	}

	public TEntity RemoveOne(TEntity entity, bool saveChanges = false)
	{
		var entry = _dbSet.Remove(entity);
		if (saveChanges)
			_context.SaveChanges();
		return entry.Entity;
	}

	public async Task<TEntity> RemoveOneAsync(Expression<Func<TEntity, bool>> filters, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		var entity = await GetOneAsync(filters, cancellationToken: cancellationToken);

		if (entity is null)
			throw new ArgumentException($"{typeof(TEntity).Name} not found", nameof(filters));

		return await RemoveOneAsync(entity, saveChanges, cancellationToken);
	}

	public TEntity RemoveOne(Expression<Func<TEntity, bool>> filters, bool saveChanges = false)
	{
		var entity = GetOne(filters);
		if (entity is null)
			throw new ArgumentException($"{typeof(TEntity).Name} not found", nameof(filters));

		return RemoveOne(entity, saveChanges);
	}

	public async Task RemoveManyAsync(IEnumerable<TEntity> entities, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		_dbSet.RemoveRange(entities);
		if (saveChanges)
			await _context.SaveChangesAsync(cancellationToken);
	}

	public void RemoveMany(IEnumerable<TEntity> entities, bool saveChanges = false)
	{
		_dbSet.RemoveRange(entities);
		if (saveChanges)
			_context.SaveChanges();
	}

	public async Task RemoveManyAsync(Expression<Func<TEntity, bool>> filters, bool saveChanges = false,
		CancellationToken cancellationToken = default)
	{
		var entities = await GetManyAsync(filters, cancellationToken: cancellationToken);
		if (!entities.Any())
			return;
		await RemoveManyAsync(entities, saveChanges, cancellationToken);
	}

	public void RemoveMany(Expression<Func<TEntity, bool>> filters, bool saveChanges = false)
	{
		var entities = GetMany(filters);
		if (!entities.Any())
			return;
		RemoveMany(entities, saveChanges);
	}

	public async Task<int> SaveAsync(CancellationToken cancellationToken = default) =>
		await _context.SaveChangesAsync(cancellationToken);

	public int Save() => _context.SaveChanges();

	
}
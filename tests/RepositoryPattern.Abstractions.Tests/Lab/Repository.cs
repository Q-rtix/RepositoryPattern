using System.Collections;
using System.Linq.Expressions;
using RepositoryPattern.Abstractions.Repositories;

namespace RepositoryPattern.Abstractions.Tests.Lab;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
	public IEnumerator<TEntity> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public Type ElementType { get; }
	public Expression Expression { get; }
	public IQueryProvider Provider { get; }

	public IQueryable<TEntity> GetMany(IEnumerable<Expression<Func<TEntity, object>>>? includes = null, bool disableTracking = false, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
		params Expression<Func<TEntity, bool>>[]? filters)
	{
		throw new NotImplementedException();
	}

	public Task<TEntity?> GetOneAsync(IEnumerable<Expression<Func<TEntity, object>>>? includes = null, bool disableTracking = false,
		CancellationToken cancellationToken = default, params Expression<Func<TEntity, bool>>[] filters)
	{
		throw new NotImplementedException();
	}

	public TEntity? GetOne(IEnumerable<Expression<Func<TEntity, object>>>? includes = null, bool disableTracking = false, params Expression<Func<TEntity, bool>>[] filters)
	{
		throw new NotImplementedException();
	}

	public Task AddOneAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void AddOne(TEntity entity)
	{
		throw new NotImplementedException();
	}

	public void AddMany(IEnumerable<TEntity> entities)
	{
		throw new NotImplementedException();
	}

	public void UpdateOne(TEntity entity)
	{
		throw new NotImplementedException();
	}

	public void UpdateMany(IEnumerable<TEntity> entities)
	{
		throw new NotImplementedException();
	}

	public void RemoveOne(TEntity entity)
	{
		throw new NotImplementedException();
	}

	public void RemoveOne(Expression<Func<TEntity, bool>>[] filters)
	{
		throw new NotImplementedException();
	}

	public void RemoveMany(IEnumerable<TEntity> entities)
	{
		throw new NotImplementedException();
	}

	public void RemoveMany(Expression<Func<TEntity, bool>>[] filters)
	{
		throw new NotImplementedException();
	}
}
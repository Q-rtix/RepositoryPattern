using RepositoryPattern.Abstractions.Repositories;
using RepositoryPattern.Abstractions.UnitOfWork;

namespace RepositoryPattern.Abstractions.Tests.Lab;

public class UnitOfWork : IUnitOfWork
{
	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public ValueTask DisposeAsync()
	{
		throw new NotImplementedException();
	}

	public bool SupportsSavePoints { get; }
	public IRepository<TEntity> Repository<TEntity>() where TEntity : class
	{
		throw new NotImplementedException();
	}

	public void BeginTransaction(bool force = false)
	{
		throw new NotImplementedException();
	}

	public Task BeginTransactionAsync(bool force = false, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void RollBack()
	{
		throw new NotImplementedException();
	}

	public Task RollBackAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void RollBackToSavePoint(string name)
	{
		throw new NotImplementedException();
	}

	public Task RollBackToSavePointAsync(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void Commit()
	{
		throw new NotImplementedException();
	}

	public Task CommitAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void CreateSavePoint(string name)
	{
		throw new NotImplementedException();
	}

	public Task CreateSavePointAsync(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void ReleaseSavePoint(string name)
	{
		throw new NotImplementedException();
	}

	public Task ReleaseSavePointAsync(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<int> SaveAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public int Save()
	{
		throw new NotImplementedException();
	}
}
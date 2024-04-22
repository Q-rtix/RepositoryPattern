using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RepositoryPattern.Abstractions.Repositories;
using RepositoryPattern.Abstractions.UnitOfWork;
using RepositoryPattern.EntityFrameworkCore.Repositories;

namespace RepositoryPattern.EntityFrameworkCore.UnitsOfWork;

internal class UnitOfWork : IUnitOfWork
{
	private readonly DbContext _context;
	private IDbContextTransaction? _transaction;
	private readonly Dictionary<string, object> _repositories;

	public UnitOfWork(DbContext context)
	{
		_context = context;
		_repositories = new Dictionary<string, object>();
	}

	public void Dispose()
	{
		TransactionDispose();
		_context.Dispose();
	}


	public async ValueTask DisposeAsync()
	{
		TransactionDisposeAsync();
		await _context.DisposeAsync();
	}

	public IRepository<TEntity> Repository<TEntity>() where TEntity : class
	{
		var entityType = typeof(TEntity);

		if (_repositories.TryGetValue(entityType.Name, out var repository))
			return (IRepository<TEntity>)repository;

		var repoType = typeof(Repository<>).MakeGenericType(entityType);
		var repo = Activator.CreateInstance(repoType, _context);

		if (repo != null)
			_repositories.Add(entityType.Name, repo);

		return (IRepository<TEntity>)_repositories[entityType.Name];
	}

	public void BeginTransaction(bool force = false)
	{
		if (_transaction is not null && !force)
			throw new InvalidOperationException("Cannot begin transaction. A transaction is already in progress.");
		
		var connection = _context.Database.GetDbConnection();
		if (connection.State is not ConnectionState.Open)
			connection.Open();

		_transaction = _context.Database.BeginTransaction(IsolationLevel.Unspecified);
	}


	public async Task BeginTransactionAsync(bool force = false, CancellationToken cancellationToken = default)
	{
		if (_transaction is not null && !force)
			throw new InvalidOperationException("Cannot begin transaction. A transaction is already in progress.");
		
		var connection = _context.Database.GetDbConnection();
		if (connection.State is not ConnectionState.Open)
			await connection.OpenAsync(cancellationToken);

		_transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);
	}

	public void RollBack()
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot roll back. No active transaction exists."); 

		_transaction.Rollback();
		TransactionDispose();
	}

	public async Task RollBackAsync(CancellationToken cancellationToken = default)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot roll back. No active transaction exists.");

		await _transaction.RollbackAsync(cancellationToken);
		TransactionDisposeAsync();
	}

	public void RollBackToSavePoint(string name)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot roll back. No active transaction exists.");

		_transaction.RollbackToSavepoint(name);
		TransactionDispose();
	}

	public async Task RollBackToSavePointAsync(string name,
		CancellationToken cancellationToken = default)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot roll back. No active transaction exists.");

		await _transaction.RollbackToSavepointAsync(name, cancellationToken);
		TransactionDisposeAsync();
	}	

	public void Commit()
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot commit. No active transaction exists.");
		
		_transaction.Commit();
		TransactionDispose();
	}

	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot commit. No active transaction exists.");

		await _transaction.CommitAsync(cancellationToken);
		TransactionDispose();
	}

	public void CreateSavePoint(string name)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot create a save point. No active transaction exists.");
		
		_transaction.CreateSavepoint(name);
	}

	public async Task CreateSavePointAsync(string name,
		CancellationToken cancellationToken = default)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot create a save point. No active transaction exists.");

		await _transaction.CreateSavepointAsync(name, cancellationToken);
	}

	public void ReleaseSavePoint(string name)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot release a save point. No active transaction exists.");
		
		_transaction.ReleaseSavepoint(name);
		TransactionDispose();
	}

	public async Task ReleaseSavePointAsync(string name,
		CancellationToken cancellationToken = default)
	{
		if (_transaction is null)
			throw new InvalidCastException("Cannot release a save point. No active transaction exists.");

		await _transaction.ReleaseSavepointAsync(name, cancellationToken);
		TransactionDisposeAsync();
	}

	public bool SupportsSavePoints => _transaction?.SupportsSavepoints ?? false;

	public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
		=> await _context.SaveChangesAsync(cancellationToken);

	public int Save() => _context.SaveChanges();

	private void TransactionDispose()
	{
		_transaction?.Dispose();
		_transaction = null;
	}

	private ValueTask? TransactionDisposeAsync()
	{
		var result = _transaction?.DisposeAsync();
		_transaction = null;
		return result;
	}
}
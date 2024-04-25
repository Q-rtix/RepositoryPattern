using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using RepositoryPattern.EntityFrameworkCore.Repositories;
using RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;
using RepositoryPattern.EntityFrameworkCore.UnitsOfWork;

namespace RepositoryPattern.EntityFrameworkCore.Tests.UnitsOfWork;

[TestSubject(typeof(UnitOfWork))]
public class UnitOfWorkTest
{
	private readonly UnitOfWork _unitOfWork;
	private readonly TestDbContext _context;

	public UnitOfWorkTest()
	{
		var options = new DbContextOptionsBuilder<TestDbContext>()
			.UseInMemoryDatabase("TestDB")
			.Options;

		_context = new TestDbContext(options);

		_unitOfWork = new UnitOfWork(_context);
	}

	[Fact]
	public void Repository_ShouldReturnNewRepositoryInstance_WhenCalled()
	{
		// Act
		var repository = _unitOfWork.Repository<TestEntity>();
		
		// Assert
		repository.Should().NotBeNull();
		repository.Should().BeOfType<Repository<TestEntity>>();
	}
	
	/*[Fact]
	public void Commit_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = () =>  _unitOfWork.Commit();
		
		// Assert
		act.Should().Throw<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}
	
	[Fact]
	public void SaveAsync_ShouldSaveChanges_WhenCalled()
	{
		// Act
		var result = _unitOfWork.SaveAsync().Result;
		
		// Assert
		result.Should().BeGreaterThan(0);
	}

	[Fact]
	public void Save_ShouldSaveChanges_WhenCalled()
	{
		// Act
		var result = _unitOfWork.Save();
		
		// Assert
		result.Should().BeGreaterThan(0);
	}

	[Fact]
	public void BeginTransaction_ShouldBeginTransaction_WhenCalled()
	{
		// Act
		_unitOfWork.BeginTransaction();
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
		_context.Database.CurrentTransaction!.Commit();
	}
	
	[Fact]
	public  void BeginTransaction_ShouldThrowAnException_WhenExistsAnActiveTransaction()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		
		// Act
		var act = () => _unitOfWork.BeginTransaction();
		
		
		// Assert
		act.Should().Throw<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().NotBeNull();
		_context.Database.CurrentTransaction!.Commit();
	}
	
	[Fact]
	public void BeginTransaction_ShouldCreateANewTransaction_WhenExistsAnActiveTransactionAndForceIsTrue()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		var oldTransactionId = _context.Database.CurrentTransaction!.TransactionId;
		
		// Act
		_unitOfWork.BeginTransaction(true);
		
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
		_context.Database.CurrentTransaction!.TransactionId.Should().NotBe(oldTransactionId);
		_context.Database.CurrentTransaction!.Commit();
	}
	
	[Fact]
	public async Task BeginTransactionAsync_ShouldBeginTransaction_WhenCalled()
	{
		// Act
		await _unitOfWork.BeginTransactionAsync();
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
		await _context.Database.CurrentTransaction!.CommitAsync();
	}
	
	[Fact]
	public async Task BeginTransactionAsync_ShouldThrowAnException_WhenExistsAnActiveTransaction()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		
		// Act
		var act = async () => await _unitOfWork.BeginTransactionAsync();
		
		
		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().NotBeNull();
		await _context.Database.CurrentTransaction!.CommitAsync();
	}
	
	[Fact]
	public async Task BeginTransactionAsync_ShouldCreateANewTransaction_WhenExistsAnActiveTransactionAndForceIsTrue()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		var oldTransactionId = _context.Database.CurrentTransaction!.TransactionId;
		
		// Act
		await _unitOfWork.BeginTransactionAsync(true);
		
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
		_context.Database.CurrentTransaction!.TransactionId.Should().NotBe(oldTransactionId);
		await _context.Database.CurrentTransaction!.CommitAsync();
	}

	[Fact]
	public void Commit_ShouldCommitTransaction_WhenCalled()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		
		// Act
		_unitOfWork.Commit();
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	
	[Fact]
	public async Task CommitAsync_ShouldCommitTransaction_WhenCalled()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		
		// Act
		await _unitOfWork.CommitAsync();
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public async Task CommitAsync_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = async () => await  _unitOfWork.CommitAsync();
		
		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}
	
	[Fact]
	public void RollBack_ShouldRollBackTransaction_WhenCalled()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		
		// Act
		_unitOfWork.RollBack();
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public void RollBack_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = () => _unitOfWork.RollBack();
		
		// Assert
		act.Should().Throw<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}
	
	[Fact]
	public async Task RollBackAsync_ShouldRollBackTransaction_WhenCalled()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		
		// Act
		await _unitOfWork.RollBackAsync();
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public async Task RollBackAsync_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = async () => await _unitOfWork.RollBackAsync();
		
		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().BeNull();
	}

	[Fact]
	public void RollBackToSavePoint_ShouldRollBackTransaction_WhenCalled()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		_unitOfWork.CreateSavePoint("test");
		
		// Act
		_unitOfWork.RollBackToSavePoint("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public void RollBackToSavePoint_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = () => _unitOfWork.RollBackToSavePoint("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}
	
	[Fact]
	public async Task RollBackToSavePointAsync_ShouldRollBackTransaction_WhenCalled()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		await _unitOfWork.CreateSavePointAsync("test");
		
		// Act
		await _unitOfWork.RollBackToSavePointAsync("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public async Task RollBackToSavePointAsync_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = async () => await _unitOfWork.RollBackToSavePointAsync("test");
		
		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().BeNull();
	}

	[Fact]
	public void CreateSavePoint_ShouldCreateSavePoint_WhenCalled()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		
		// Act
		_unitOfWork.CreateSavePoint("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}
	
	[Fact]
	public void CreateSavePoint_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = () => _unitOfWork.CreateSavePoint("test");
		
		// Assert
		act.Should().Throw<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public async Task CreateSavePointAsync_ShouldCreateSavePoint_WhenCalled()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		
		// Act
		await _unitOfWork.CreateSavePointAsync("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}
	
	[Fact]
	public async Task CreateSavePointAsync_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = async () => await _unitOfWork.CreateSavePointAsync("test");
		
		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().NotBeNull();
	}

	[Fact]
	public void ReleaseSavePoint_ShouldReleaseSavePoint_WhenCalled()
	{
		// Arrange
		_unitOfWork.BeginTransaction();
		_unitOfWork.CreateSavePoint("test");
		
		// Act
		_unitOfWork.ReleaseSavePoint("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public void ReleaseSavePoint_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = () => _unitOfWork.ReleaseSavePoint("test");
		
		// Assert
		act.Should().Throw<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public async Task ReleaseSavePointAsync_ShouldReleaseSavePoint_WhenCalled()
	{
		// Arrange
		await _unitOfWork.BeginTransactionAsync();
		await _unitOfWork.CreateSavePointAsync("test");
		
		// Act
		await _unitOfWork.ReleaseSavePointAsync("test");
		
		// Assert
		_context.Database.CurrentTransaction.Should().BeNull();
	}
	
	[Fact]
	public async Task ReleaseSavePointAsync_ShouldThrowAnException_WhenNotExistsAnActiveTransaction()
	{
		// Act
		var act = async () => await _unitOfWork.ReleaseSavePointAsync("test");
		
		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>();
		_context.Database.CurrentTransaction.Should().BeNull();
	}*/
}
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RepositoryPattern.Abstractions.Extensions;
using RepositoryPattern.EntityFrameworkCore.Extensions;
using RepositoryPattern.EntityFrameworkCore.Repositories;
using RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

namespace RepositoryPattern.EntityFrameworkCore.Tests.Repositories;

public class RepositoryOfTEntityTests
{
	private readonly TestDbContext _context;
	private readonly Repository<TestEntity> _repository;

	public RepositoryOfTEntityTests()
	{
		var options = new DbContextOptionsBuilder<TestDbContext>()
			.UseInMemoryDatabase("TestDB")
			.Options;

		_context = new TestDbContext(options);

		_repository = new Repository<TestEntity>(_context);
	}

	[Fact]
	public void Where_ShouldReturnCorrectEntities()
	{
		// Arrange
		_context.TestEntities.Add(new TestEntity { Id = 1121, Name = "Test 111" });
		_context.TestEntities.Add(new TestEntity { Id = 2212, Name = "Test 222" });
		_context.SaveChanges();

		// Act
		var result = _repository
			.Where(e => e.Id == 1121)
			.ToList();

		// Assert
		result.Should().ContainSingle(e => e.Id == 1121);
	}

	[Fact]
	public void FirstOrDefault_ShouldReturnCorrectEntity()
	{
		// Arrange
		_context.TestEntities.Add(new TestEntity { Id = 111, Name = "Test 111" });
		_context.TestEntities.Add(new TestEntity { Id = 222, Name = "Test 222" });
		_context.SaveChanges();

		// Act
		var result = _repository
			.FirstOrDefault(e => e.Id == 111);

		// Assert
		result.Should().BeEquivalentTo(new TestEntity { Id = 111, Name = "Test 111" });
	}

	[Fact]
	public async Task FirstOrDefaultAsync_ShouldReturnCorrectEntity()
	{
		// Arrange
		_context.TestEntities.Add(new TestEntity { Id = 1211, Name = "Test 111" });
		_context.TestEntities.Add(new TestEntity { Id = 2122, Name = "Test 222" });
		_context.SaveChanges();

		// Act
		var result = await _repository
			.FirstOrDefaultAsync(e => e.Id == 1211);

		// Assert
		result.Should().BeEquivalentTo(new TestEntity { Id = 1211, Name = "Test 111" });
	}

	[Fact]
	public void GetMany_NoFilters_ReturnsAll()
	{
		// Arrange
		var entities = _context.TestEntities.ToList();

		// Act
		var result = _repository.GetMany();

		// Assert
		result.Should().BeEquivalentTo(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetMany_WithFilter_FiltersCorrectly()
	{
		// Arrange
		_context.TestEntities.Add(new TestEntity { Id = 1111, Name = "Test 111" });
		_context.SaveChanges();

		// Act
		var result = _repository.GetMany(filters: e => e.Id == 1111);

		// Assert
		result.Should().ContainSingle(e => e.Id == 1111);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetMany_WithOrderBy_OrdersCorrectly()
	{
		// Arrange
		var entities = _repository.ToList();

		// Act
		var result = _repository.GetMany(orderBy: q => q.OrderBy(e => e.Id));

		// Assert
		result.Should().BeInAscendingOrder(e => e.Id);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetMany_DisableTracking_DoesNotTrackEntities()
	{
		// Act
		var result = _repository.GetMany(disableTracking: true);

		// Assert
		result.Should().NotBeNull();
		result.AsEnumerable().All(e => _context.Entry(e).State == EntityState.Detached).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetMany_WithIncludes_IncludesRelatedEntities()
	{
		// Arrange
		List<RelatedTestEntity> relatedTestEntities =
		[
			new RelatedTestEntity { Id = 10, Name = "Related 1", TestEntityId = 10 },
			new RelatedTestEntity { Id = 20, Name = "Related 2", TestEntityId = 20 },
			new RelatedTestEntity { Id = 30, Name = "Related 3", TestEntityId = 20 },
			new RelatedTestEntity { Id = 40, Name = "Related 4", TestEntityId = 30 },
			new RelatedTestEntity { Id = 50, Name = "Related 5", TestEntityId = 30 },
			new RelatedTestEntity { Id = 60, Name = "Related 6", TestEntityId = 30 }
		];
		IEnumerable<TestEntity> testEntities =
		[
			new TestEntity { Id = 10, Name = "Test 1", RelatedTestEntities = [relatedTestEntities[0]] },
			new TestEntity
				{ Id = 20, Name = "Test 2", RelatedTestEntities = [relatedTestEntities[1], relatedTestEntities[2]] },
			new TestEntity
			{
				Id = 30, Name = "Test 3",
				RelatedTestEntities = [relatedTestEntities[3], relatedTestEntities[4], relatedTestEntities[5]]
			},
		];
		_context.AddRange(relatedTestEntities);
		_context.AddRange(testEntities);
		_context.SaveChanges();
		var entities = _repository.Include(e => e.RelatedTestEntities).ToList();
		var includes = new Expression<Func<TestEntity, object>>[]
		{
			e => e.RelatedTestEntities
				.Then(r => r.InsideRelatedTestEntity)
		};

		// Act
		var result = _repository.GetMany(includes: includes);

		// Assert
		result.Should().BeEquivalentTo(entities);
		result.AsEnumerable().Any(e => e.RelatedTestEntities.Count != 0).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetMany_WithThenIncludes_IncludesRelatedEntities()
	{
		// Arrange
		List<RelatedTestEntity> relatedTestEntities =
		[
			new RelatedTestEntity { Id = 1, Name = "Related 1", TestEntityId = 1, InsideRelatedTestEntityId = 1},
			new RelatedTestEntity { Id = 2, Name = "Related 2", TestEntityId = 2 },
			new RelatedTestEntity { Id = 3, Name = "Related 3", TestEntityId = 2, InsideRelatedTestEntityId = 1 },
			new RelatedTestEntity { Id = 4, Name = "Related 4", TestEntityId = 3, InsideRelatedTestEntityId = 1 },
			new RelatedTestEntity { Id = 5, Name = "Related 5", TestEntityId = 3 },
			new RelatedTestEntity { Id = 6, Name = "Related 6", TestEntityId = 3, InsideRelatedTestEntityId = 1 }
		];
		InsideRelatedTestEntity insideRelatedEntity = new() 
		{
			Id = 1, 
			RelatedTestEntities = [
				relatedTestEntities[0],
				relatedTestEntities[2],
				relatedTestEntities[3],
				relatedTestEntities[5],
			]
		};
		IEnumerable<TestEntity> testEntities =
		[
			new TestEntity { Id = 1, Name = "Test 1", RelatedTestEntities = [relatedTestEntities[0]] },
			new TestEntity
				{ Id = 2, Name = "Test 2", RelatedTestEntities = [relatedTestEntities[1], relatedTestEntities[2]] },
			new TestEntity
			{
				Id = 3, Name = "Test 3",
				RelatedTestEntities = [relatedTestEntities[3], relatedTestEntities[4], relatedTestEntities[5]]
			},
		];
		_context.Add(insideRelatedEntity);
		_context.AddRange(relatedTestEntities);
		_context.AddRange(testEntities);
		_context.SaveChanges();
		var entities = _repository.Include(e => e.RelatedTestEntities)
			.ThenInclude(r => r.InsideRelatedTestEntity).ToList();
		
		var includes = new Expression<Func<TestEntity, object>>[] { 
			e => e.RelatedTestEntities.Then(r => r.InsideRelatedTestEntity)
		};

		// Act
		var result = _repository.GetMany(includes: includes);

		// Assert
		result.Should().BeEquivalentTo(entities);
		result.AsEnumerable().Any(e => e.RelatedTestEntities.Count != 0).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_ExistingEntity_ReturnsEntity()
	{
		// Arrange
		var entity = _repository.First();

		// Act
		var result = await _repository.GetOneAsync();

		// Assert
		result.Should().BeEquivalentTo(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_NonExistingEntity_ReturnsNull()
	{
		// Arrange
		var nonExistingId = _repository.Max(e => e.Id) + 1;

		// Act
		var result = await _repository.GetOneAsync(filters: e => e.Id == nonExistingId);

		// Assert
		result.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_DisableTracking_DoesNotTrackEntity()
	{
		// Arrange
		var entity = _repository.First();

		// Act
		var result = await _repository.GetOneAsync(disableTracking: true, filters: e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);
		(_context.Entry(result).State == EntityState.Detached).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_WithIncludes_IncludesRelatedEntities()
	{
		// Arrange
		var entity = _repository.Include(e => e.RelatedTestEntities).First();
		var includes = new Expression<Func<TestEntity, object>>[] { e => e.RelatedTestEntities };

		// Act
		var result = await _repository.GetOneAsync(includes: includes, filters: e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);
		result.RelatedTestEntities.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_WithCancellationToken_HonorsCancellation()
	{
		// Arrange
		var cts = new CancellationTokenSource();
		cts.Cancel();

		// Act
		Func<Task> action = async () =>
			await _repository.GetOneAsync(cancellationToken: cts.Token, filters: e => e.Id == 1);

		// Assert
		await action.Should().ThrowAsync<OperationCanceledException>();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_ExistingEntity_ReturnsEntity()
	{
		// Arrange
		var entity = _repository.First();

		// Act
		var result = _repository.GetOne(filters: e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_NonExistingEntity_ReturnsNull()
	{
		// Act
		var result = _repository.GetOne(filters: e => e.Id == 999);

		// Assert
		result.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_DisableTracking_DoesNotTrackEntity()
	{
		// Arrange
		var entity = _repository.First();

		// Act
		var result = _repository.GetOne(disableTracking: true, filters: e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);
		(_context.Entry(result).State == EntityState.Detached).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_WithIncludes_IncludesRelatedEntities()
	{
		// Arrange
		var entity = _repository.Include(e => e.RelatedTestEntities).First();
		var includes = new Expression<Func<TestEntity, object>>[] { e => e.RelatedTestEntities };

		// Act
		var result = _repository.GetOne(includes: includes, filters: e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);
		result.RelatedTestEntities.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task AddOneAsync_AddsEntityToContext()
	{
		// Arrange
		var entity = new TestEntity { Id = 5, Name = "Test 4" };

		// Act
		await _repository.AddOneAsync(entity);

		// Assert
		_context.Entry(entity).State.Should().Be(EntityState.Added);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddOne_ShouldAddEntityToContext()
	{
		// Arrange
		var entity = new TestEntity { Id = 612, Name = "Test Entity" };

		// Act
		_repository.AddOne(entity);

		// Assert
		_context.Entry(entity).State.Should().Be(EntityState.Added);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task AddManyAsync_ShouldAddEntitiesToContext()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 64, Name = "Test Entity 1" },
			new() { Id = 74, Name = "Test Entity 2" }
		};

		// Act
		await _repository.AddManyAsync(entities);

		// Assert
		_context.Entry(entities[0]).State.Should().Be(EntityState.Added);
		_context.Entry(entities[1]).State.Should().Be(EntityState.Added);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddMany_ShouldAddEntitiesToContext()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 62, Name = "Test Entity 1" },
			new() { Id = 72, Name = "Test Entity 2" }
		};

		// Act
		_repository.AddMany(entities);

		// Assert
		_context.Entry(entities[0]).State.Should().Be(EntityState.Added);
		_context.Entry(entities[1]).State.Should().Be(EntityState.Added);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateOne_ShouldUpdateEntityInContext()
	{
		// Arrange
		var entity = new TestEntity { Id = 776, Name = "Test Entity" };
		_repository.AddOne(entity);

		entity.Name = "Updated Test Entity";

		// Act
		_repository.UpdateOne(entity);

		// Assert
		_context.Entry(entity).State.Should().Be(EntityState.Modified);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateMany_ShouldSaveChangesToContext()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 456, Name = "Test Entity 1" },
			new() { Id = 457, Name = "Test Entity 2" }
		};
		_repository.AddMany(entities);

		entities[0].Name = "Updated Test Entity 1";
		entities[1].Name = "Updated Test Entity 2";

		// Act
		_repository.UpdateMany(entities);

		// Assert
		_context.Entry(entities[0]).State.Should().Be(EntityState.Modified);
		_context.Entry(entities[1]).State.Should().Be(EntityState.Modified);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_ShouldRemoveEntityFromContext()
	{
		// Arrange
		var entity = new TestEntity { Id = 608, Name = "Test Entity" };
		_repository.AddOne(entity);
		_context.SaveChanges();

		// Act
		_repository.RemoveOne(entity);

		// Assert
		_context.Entry(entity).State.Should().Be(EntityState.Deleted);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_UsingFilters_ShouldRemoveEntityFromContext()
	{
		// Arrange
		var entity = new TestEntity { Id = 657, Name = "Test Entity" };
		_repository.AddOne(entity);
		_context.SaveChanges();

		// Act
		_repository.RemoveOne([e => e.Id == entity.Id]);

		// Assert
		_context.Entry(entity).State.Should().Be(EntityState.Deleted);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveMany_UsingFilters_ShouldRemoveEntitiesFromDbSet_WhenEntitiesExist()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 81, Name = "Entity 1" },
			new() { Id = 82, Name = "Entity 2" }
		};
		_repository.AddMany(entities);
		_context.SaveChanges();

		// Act
		_repository.RemoveMany([e => entities.Select(testEntity => testEntity.Id).Contains(e.Id)]);

		// Assert
		_context.TestEntities.Where(e => e.Id == 81 || e.Id == 82)
			.ToList()
			.All(e => _context.Entry(e).State == EntityState.Deleted).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}
}
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
	public void Data_Returns_DbSet()
	{
		// Act
		var result = _repository.Data;

		// Assert
#pragma warning disable EF1001
		result.Should().NotBeNull()
			.And.Equal(_context.Set<TestEntity>())
			.And.BeOfType<InternalDbSet<TestEntity>>();
#pragma warning restore EF1001

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void Data_Throws_IfDbSetNull()
	{
		// Arrange
		Repository<TestEntity>? repo = null;

		// Act
		var act = () =>
		{
			var result = repo.Data;
		};

		// Assert
		act.Should().Throw<NullReferenceException>();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetManyAsync_NoFilters_ReturnsAll()
	{
		// Act
		var result = await _repository.GetManyAsync();

		// Assert
		result.Should().BeEquivalentTo(_context.Set<TestEntity>());

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetManyAsync_WithFilter_FiltersCorrectly()
	{
		// Arrange
		_context.Set<TestEntity>().Add(new TestEntity { Id = 1111, Name = "Test 1111" });
		await _context.SaveChangesAsync();
		
		// Act
		var result = await _repository.GetManyAsync(e => e.Id == 1111);

		// Assert
		result.Count().Should().Be(1);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetManyAsync_WithOrderBy_OrdersCorrectly()
	{
		// Act
		var result = await _repository.GetManyAsync(orderBy: q => q.OrderBy(e => e.Id));

		// Assert
		result.Should().BeInAscendingOrder(e => e.Id);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetManyAsync_DisableTracking_DoesNotTrackEntities()
	{
		// Arrange
		var entities = await _repository.Data.ToListAsync();

		// Act
		var result = await _repository.GetManyAsync(disableTracking: true);

		// Assert
		result.AsEnumerable().All(e => _context.Entry(e).State == EntityState.Detached).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetManyAsync_WithIncludes_IncludesRelatedEntities()
	{
		// Arrange
		List<RelatedTestEntity> relatedTestEntities =
		[
			new RelatedTestEntity { Id = 91, Name = "Related 1", TestEntityId = 1 },
			new RelatedTestEntity { Id = 92, Name = "Related 2", TestEntityId = 2 },
			new RelatedTestEntity { Id = 93, Name = "Related 3", TestEntityId = 2 },
			new RelatedTestEntity { Id = 94, Name = "Related 4", TestEntityId = 3 },
			new RelatedTestEntity { Id = 95, Name = "Related 5", TestEntityId = 3 },
			new RelatedTestEntity { Id = 96, Name = "Related 6", TestEntityId = 3 }
		];
		IEnumerable<TestEntity> testEntities =
		[
			new TestEntity { Id = 91, Name = "Test 1", RelatedTestEntities = [relatedTestEntities[0]] },
			new TestEntity
				{ Id = 92, Name = "Test 2", RelatedTestEntities = [relatedTestEntities[1], relatedTestEntities[2]] },
			new TestEntity
			{
				Id = 93, Name = "Test 3",
				RelatedTestEntities = [relatedTestEntities[3], relatedTestEntities[4], relatedTestEntities[5]]
			},
		];
		await _context.AddRangeAsync(relatedTestEntities);
		await _context.AddRangeAsync(testEntities);
		await _context.SaveChangesAsync();
		var includes = new Expression<Func<TestEntity, object>>[] { e => e.RelatedTestEntities };

		// Act
		var result = await _repository.GetManyAsync(includes: includes);

		// Assert
		result.Any(e => e.RelatedTestEntities.Count != 0).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetManyAsync_WithCancellationToken_HonorsCancellation()
	{
		// Arrange
		var cts = new CancellationTokenSource();
		cts.Cancel();

		// Act
		Func<Task> action = async () => await _repository.GetManyAsync(cancellationToken: cts.Token);

		// Assert
		await action.Should().ThrowAsync<OperationCanceledException>();

		_context.ChangeTracker.Clear();
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
		_context.TestEntities.Add(new TestEntity{Id = 111, Name = "Test 111"});
		_context.SaveChanges();

		// Act
		var result = _repository.GetMany(e => e.Id == 111);

		// Assert
		result.Should().ContainSingle(e => e.Id == 111);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetMany_WithOrderBy_OrdersCorrectly()
	{
		// Arrange
		var entities = _repository.Data.ToList();

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
			new RelatedTestEntity { Id = 1, Name = "Related 1", TestEntityId = 1 },
			new RelatedTestEntity { Id = 2, Name = "Related 2", TestEntityId = 2 },
			new RelatedTestEntity { Id = 3, Name = "Related 3", TestEntityId = 2 },
			new RelatedTestEntity { Id = 4, Name = "Related 4", TestEntityId = 3 },
			new RelatedTestEntity { Id = 5, Name = "Related 5", TestEntityId = 3 },
			new RelatedTestEntity { Id = 6, Name = "Related 6", TestEntityId = 3 }
		];
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
		_context.AddRange(relatedTestEntities);
		_context.AddRange(testEntities);
		_context.SaveChanges();
		var entities = _repository.Data.Include(e => e.RelatedTestEntities).ToList();
		var includes = new Expression<Func<TestEntity, object>>[] { e => e.RelatedTestEntities };

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
		var entity = _repository.Data.First();

		// Act
		var result = await _repository.GetOneAsync(e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_NonExistingEntity_ReturnsNull()
	{
		// Arrange
		var nonExistingId = _repository.Data.Max(e => e.Id) + 1;

		// Act
		var result = await _repository.GetOneAsync(e => e.Id == nonExistingId);

		// Assert
		result.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_DisableTracking_DoesNotTrackEntity()
	{
		// Arrange
		var entity = _repository.Data.First();

		// Act
		var result = await _repository.GetOneAsync(e => e.Id == entity.Id, true);

		// Assert
		result.Should().BeEquivalentTo(entity);
		(_context.Entry(result).State == EntityState.Detached).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task GetOneAsync_WithIncludes_IncludesRelatedEntities()
	{
		// Arrange
		var entity = _repository.Data.Include(e => e.RelatedTestEntities).First();
		var includes = new Expression<Func<TestEntity, object>>[] { e => e.RelatedTestEntities };

		// Act
		var result = await _repository.GetOneAsync(e => e.Id == entity.Id, includes: includes);

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
		Func<Task> action = async () => await _repository.GetOneAsync(e => e.Id == 1, cancellationToken: cts.Token);

		// Assert
		await action.Should().ThrowAsync<OperationCanceledException>();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_ExistingEntity_ReturnsEntity()
	{
		// Arrange
		var entity = _repository.Data.First();

		// Act
		var result = _repository.GetOne(e => e.Id == entity.Id);

		// Assert
		result.Should().BeEquivalentTo(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_NonExistingEntity_ReturnsNull()
	{
		// Act
		var result = _repository.GetOne(e => e.Id == 999);

		// Assert
		result.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_DisableTracking_DoesNotTrackEntity()
	{
		// Arrange
		var entity = _repository.Data.First();

		// Act
		var result = _repository.GetOne(e => e.Id == entity.Id, true);

		// Assert
		result.Should().BeEquivalentTo(entity);
		(_context.Entry(result).State == EntityState.Detached).Should().BeTrue();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void GetOne_WithIncludes_IncludesRelatedEntities()
	{
		// Arrange
		var entity = _repository.Data.Include(e => e.RelatedTestEntities).First();
		var includes = new Expression<Func<TestEntity, object>>[] { e => e.RelatedTestEntities };

		// Act
		var result = _repository.GetOne(e => e.Id == entity.Id, includes: includes);

		// Assert
		result.Should().BeEquivalentTo(entity);
		result.RelatedTestEntities.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task AddOneAsync_WithoutSaveChanges_AddsEntityToContext()
	{
		// Arrange
		var entity = new TestEntity { Id = 5, Name = "Test 4" };

		// Act
		var result = await _repository.AddOneAsync(entity, false);

		// Assert
		result.Should().BeEquivalentTo(entity);
		_context.Entry(result).State.Should().Be(EntityState.Added);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task AddOneAsync_WithSaveChanges_AddsEntityToDatabase()
	{
		// Arrange
		var entity = new TestEntity { Id = 50, Name = "Test 5" };

		// Act
		var result = await _repository.AddOneAsync(entity, true);

		// Assert
		result.Should().BeEquivalentTo(entity);
		_context.Entry(result).State.Should().Be(EntityState.Unchanged);
		_repository.Data.Should().Contain(result);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddOne_ShouldAddEntityToDbSet()
	{
		// Arrange
		var entity = new TestEntity { Id = 612, Name = "Test Entity" };

		// Act
		var addedEntity = _repository.AddOne(entity);

		// Assert
		addedEntity.Should().BeSameAs(entity);
		_repository.Data.Should()
			.Contain(e => e == entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddOne_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 66, Name = "Test Entity" };

		// Act
		_repository.AddOne(entity, true);

		// Assert
		_repository.Data.Should()
			.Contain(e => e == entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddOne_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 65, Name = "Test Entity" };

		// Act
		_repository.AddOne(entity, false);

		// Assert
		_repository.Data.Should()
			.NotContain(e => e == entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task AddManyAsync_ShouldAddEntitiesToDbSet()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 63, Name = "Test Entity 1" },
			new() { Id = 73, Name = "Test Entity 2" }
		};

		// Act
		await _repository.AddManyAsync(entities);

		// Assert
		_repository.Data.Should()
			.Contain(e => e == entities[0]);
		_repository.Data.Should()
			.Contain(e => e == entities[1]);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task AddManyAsync_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 619, Name = "Test Entity 1" },
			new() { Id = 719, Name = "Test Entity 2" }
		};

		// Act
		await _repository.AddManyAsync(entities, true);

		// Assert
		_repository.Data.Should()
			.Contain(e => e == entities[0]);
		_repository.Data.Should()
			.Contain(e => e == entities[1]);

		await _repository.SaveAsync();
	}

	[Fact]
	public async Task AddManyAsync_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 64, Name = "Test Entity 1" },
			new() { Id = 74, Name = "Test Entity 2" }
		};

		// Act
		await _repository.AddManyAsync(entities, false);

		// Assert
		_repository.Data.Should()
			.NotContain(e => e == entities[0]);
		_repository.Data.Should()
			.NotContain(e => e == entities[1]);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddMany_ShouldAddEntitiesToDbSet()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 60, Name = "Test Entity 1" },
			new() { Id = 70, Name = "Test Entity 2" }
		};

		// Act
		_repository.AddMany(entities);

		// Assert
		_repository.Data.Should()
			.Contain(e => e == entities[0]);
		_repository.Data.Should()
			.Contain(e => e == entities[1]);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddMany_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 62, Name = "Test Entity 1" },
			new() { Id = 72, Name = "Test Entity 2" }
		};

		// Act
		_repository.AddMany(entities, true);

		// Assert
		_repository.Data.Should()
			.Contain(e => e == entities[0]);
		_repository.Data.Should()
			.Contain(e => e == entities[1]);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void AddMany_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 61, Name = "Test Entity 1" },
			new() { Id = 71, Name = "Test Entity 2" }
		};

		// Act
		_repository.AddMany(entities, false);

		// Assert
		_repository.Data.Should()
			.NotContain(e => e == entities[0]);
		_repository.Data.Should()
			.NotContain(e => e == entities[1]);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task UpdateOneAsync_ShouldUpdateEntityInDbSet()
	{
		// Arrange
		var entity = new TestEntity { Id = 796, Name = "Test Entity" };
		await _repository.AddOneAsync(entity, true);

		entity.Name = "Updated Test Entity";

		// Act
		var updatedEntity = await _repository.UpdateOneAsync(entity);

		// Assert
		_repository.Data.Should().Contain(updatedEntity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task UpdateOneAsync_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 665, Name = "Test Entity" };
		await _repository.AddOneAsync(entity, true);

		entity.Name = "Updated Test Entity";

		// Act
		await _repository.UpdateOneAsync(entity, true);

		// Assert
		var updatedEntity = _context.Set<TestEntity>().FirstOrDefault(e => e.Id == entity.Id);
		updatedEntity.Should().Be(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task UpdateOneAsync_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 613, Name = "Test Entity" };
		await _repository.AddOneAsync(entity, true);

		entity.Name = "Updated Test Entity";

		// Act
		await _repository.UpdateOneAsync(entity, false);

		// Assert
		var updatedEntity = _repository.Data.FirstOrDefault(e => e.Id == entity.Id);
		updatedEntity.Should().NotBe(new TestEntity { Id = 613, Name = "Test Entity" });

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateOne_ShouldUpdateEntityInDbSet()
	{
		// Arrange
		var entity = new TestEntity { Id = 776, Name = "Test Entity" };
		_repository.AddOne(entity, true);

		entity.Name = "Updated Test Entity";

		// Act
		var updatedEntity = _repository.UpdateOne(entity);

		// Assert
		_repository.Data.Should().Contain(updatedEntity);
		_repository.Data.Should().NotContain(new TestEntity { Id = 776, Name = "Test Entity" });

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateOne_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 656, Name = "Test Entity" };
		_repository.AddOne(entity, true);

		entity.Name = "Updated Test Entity";

		// Act
		_repository.UpdateOne(entity, true);

		// Assert
		var updatedEntity = _context.Set<TestEntity>().FirstOrDefault(e => e.Id == entity.Id);
		updatedEntity.Should().Be(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateOne_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 666, Name = "Test Entity" };
		_repository.AddOne(entity, true);

		entity.Name = "Updated Test Entity";

		// Act
		_repository.UpdateOne(entity, false);

		// Assert
		var updatedEntity = _repository.Data.FirstOrDefault(e => e.Id == entity.Id);
		updatedEntity.Should().NotBe(new TestEntity { Id = 666, Name = "Test Entity" });

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task UpdateManyAsync_ShouldUpdateEntitiesInDbSet()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 256, Name = "Test Entity 1" },
			new() { Id = 257, Name = "Test Entity 2" }
		};
		await _repository.AddManyAsync(entities);

		entities[0].Name = "Updated Test Entity 1";
		entities[1].Name = "Updated Test Entity 2";

		// Act
		await _repository.UpdateManyAsync(entities);

		// Assert
		_repository.Data.Should().Contain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task UpdateManyAsync_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 536, Name = "Test Entity 1" },
			new() { Id = 537, Name = "Test Entity 2" }
		};
		await _repository.AddManyAsync(entities);

		entities[0].Name = "Updated Test Entity 1";
		entities[1].Name = "Updated Test Entity 2";

		// Act
		await _repository.UpdateManyAsync(entities, true);

		// Assert
		var updatedEntities = _context.Set<TestEntity>().ToList();
		updatedEntities.Should().Contain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task UpdateManyAsync_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 356, Name = "Test Entity 1" },
			new() { Id = 357, Name = "Test Entity 2" }
		};
		await _repository.AddManyAsync(entities);

		entities[0].Name = "Updated Test Entity 1";
		entities[1].Name = "Updated Test Entity 2";

		// Act
		await _repository.UpdateManyAsync(entities, false);

		// Assert
		_repository.Data.Should().NotContain(new List<TestEntity>
		{
			new() { Id = 356, Name = "Test Entity 1" },
			new() { Id = 357, Name = "Test Entity 2" }
		});

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateMany_ShouldUpdateEntitiesInDbSet()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 546, Name = "Test Entity 1" },
			new() { Id = 547, Name = "Test Entity 2" }
		};
		_repository.AddMany(entities);

		entities[0].Name = "Updated Test Entity 1";
		entities[1].Name = "Updated Test Entity 2";

		// Act
		_repository.UpdateMany(entities);

		// Assert
		_repository.Data.Should().Contain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateMany_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
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
		_repository.UpdateMany(entities, true);

		// Assert
		var updatedEntities = _context.Set<TestEntity>().ToList();
		updatedEntities.Should().Contain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void UpdateMany_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 690, Name = "Test Entity 1" },
			new() { Id = 790, Name = "Test Entity 2" }
		};
		_repository.AddMany(entities);

		entities[0].Name = "Updated Test Entity 1";
		entities[1].Name = "Updated Test Entity 2";

		// Act
		_repository.UpdateMany(entities, false);

		// Assert
		_repository.Data.Should().NotContain(new List<TestEntity>
		{
			new() { Id = 690, Name = "Test Entity 1" },
			new() { Id = 790, Name = "Test Entity 2" }
		});

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_ShouldRemoveEntityFromDbSet()
	{
		// Arrange
		var entity = new TestEntity { Id = 641, Name = "Test Entity" };
		await _repository.AddOneAsync(entity);

		// Act
		var removedEntity = await _repository.RemoveOneAsync(entity);

		// Assert
		_repository.Data.Should().NotContain(removedEntity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 167, Name = "Test Entity" };
		await _repository.AddOneAsync(entity);

		// Act
		await _repository.RemoveOneAsync(entity, true);

		// Assert
		var removedEntity = _context.Set<TestEntity>().Find(entity.Id);
		removedEntity.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 643, Name = "Test Entity" };
		await _repository.AddOneAsync(entity);

		// Act
		await _repository.RemoveOneAsync(entity, false);

		// Assert
		var removedEntity = _context.Set<TestEntity>().Find(entity.Id);
		removedEntity.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_ShouldRemoveEntityFromDbSet()
	{
		// Arrange
		var entity = new TestEntity { Id = 608, Name = "Test Entity" };
		_repository.AddOne(entity);

		// Act
		var removedEntity = _repository.RemoveOne(entity);

		// Assert
		_repository.Data.Should().NotContain(removedEntity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 600, Name = "Test Entity" };
		_repository.AddOne(entity);

		// Act
		_repository.RemoveOne(entity, true);

		// Assert
		var removedEntity = _context.Set<TestEntity>().Find(entity.Id);
		removedEntity.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 605, Name = "Test Entity" };
		_repository.AddOne(entity);

		// Act
		_repository.RemoveOne(entity, false);

		// Assert
		var removedEntity = _context.Set<TestEntity>().FirstOrDefault(e => e.Id == entity.Id);
		removedEntity.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_UsingFilters_ShouldRemoveEntityFromDbSet_WhenEntityExists()
	{
		// Arrange
		var entity = new TestEntity { Id = 166, Name = "Test Entity" };
		await _repository.AddOneAsync(entity);

		// Act
		var removedEntity = await _repository.RemoveOneAsync(e => e.Id == entity.Id);

		// Assert
		_repository.Data.Should().NotContain(removedEntity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_UsingFilters_ShouldThrowArgumentException_WhenEntityDoesNotExist()
	{
		// Arrange
		var entity = new TestEntity { Id = 186, Name = "Test Entity" };

		// Act
		Func<Task> act = () => _repository.RemoveOneAsync(e => e.Id == entity.Id);

		// Assert
		await act.Should().ThrowAsync<ArgumentException>()
			.WithMessage($"{nameof(TestEntity)} not found (Parameter 'filters')");

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_UsingFilters_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 176, Name = "Test Entity" };
		await _repository.AddOneAsync(entity);

		// Act
		await _repository.RemoveOneAsync(e => e.Id == entity.Id, true);

		// Assert
		var removedEntity = await _context.Set<TestEntity>().FindAsync(entity.Id);
		removedEntity.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveOneAsync_UsingFilters_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 146, Name = "Test Entity" };
		await _repository.AddOneAsync(entity);

		// Act
		await _repository.RemoveOneAsync(e => e.Id == entity.Id, false);

		// Assert
		var removedEntity = await _context.Set<TestEntity>().FindAsync(entity.Id);
		removedEntity.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_UsingFilters_ShouldRemoveEntityFromDbSet_WhenEntityExists()
	{
		// Arrange
		var entity = new TestEntity { Id = 657, Name = "Test Entity" };
		_repository.AddOne(entity);

		// Act
		var removedEntity = _repository.RemoveOne(e => e.Id == entity.Id);

		// Assert
		_repository.Data.Should().NotContain(removedEntity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_UsingFilters_ShouldThrowArgumentException_WhenEntityDoesNotExist()
	{
		// Arrange
		var entity = new TestEntity { Id = 676, Name = "Test Entity" };

		// Act
		Action act = () => _repository.RemoveOne(e => e.Id == entity.Id);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage($"{nameof(TestEntity)} not found (Parameter 'filters')");

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_UsingFilters_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entity = new TestEntity { Id = 665, Name = "Test Entity" };
		_repository.AddOne(entity);

		// Act
		_repository.RemoveOne(e => e.Id == entity.Id, true);

		// Assert
		var removedEntity = _context.Set<TestEntity>().Find(entity.Id);
		removedEntity.Should().BeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveOne_UsingFilters_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entity = new TestEntity { Id = 695, Name = "Test Entity" };
		_repository.AddOne(entity);

		// Act
		_repository.RemoveOne(e => e.Id == entity.Id, false);

		// Assert
		var removedEntity = _context.Set<TestEntity>().Find(entity.Id);
		removedEntity.Should().NotBeNull();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveManyAsync_ShouldRemoveEntitiesFromDbSet_WhenEntitiesExist()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 109, Name = "Entity 1" },
			new() { Id = 209, Name = "Entity 2" }
		};
		await _repository.AddManyAsync(entities);

		// Act
		await _repository.RemoveManyAsync(entities);

		// Assert
		_repository.Data.Should().NotContain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveManyAsync_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 108, Name = "Entity 1" },
			new() { Id = 208, Name = "Entity 2" }
		};
		await _repository.AddManyAsync(entities);

		// Act
		await _repository.RemoveManyAsync(entities, true);

		// Assert
		var removedEntities = _context.Set<TestEntity>().Where(e => entities.Select(e => e.Id).Contains(e.Id));
		removedEntities.Should().BeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveManyAsync_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 100, Name = "Entity 1" },
			new() { Id = 200, Name = "Entity 2" }
		};
		await _repository.AddManyAsync(entities);

		// Act
		await _repository.RemoveManyAsync(entities, false);

		// Assert
		var removedEntities = _context.Set<TestEntity>()
			.Where(e => e.Id == entities[0].Id || e.Id == entities[1].Id);
		removedEntities.Should().NotBeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveMany_ShouldRemoveEntitiesFromDbSet_WhenEntitiesExist()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 21, Name = "Entity 1" },
			new() { Id = 22, Name = "Entity 2" }
		};
		_repository.AddMany(entities);

		// Act
		_repository.RemoveMany(entities);

		// Assert
		_repository.Data.Should().NotContain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveMany_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 31, Name = "Entity 1" },
			new() { Id = 32, Name = "Entity 2" }
		};
		_repository.AddMany(entities);

		// Act
		_repository.RemoveMany(entities, true);

		// Assert
		var removedEntities = _context.Set<TestEntity>().Where(e => entities.Select(e => e.Id).Contains(e.Id));
		removedEntities.Should().BeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveMany_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 11, Name = "Entity 1" },
			new() { Id = 12, Name = "Entity 2" }
		};
		_repository.AddMany(entities);

		// Act
		_repository.RemoveMany(entities, false);

		// Assert
		_context.Set<TestEntity>()
			.Where(e => e.Id == entities[0].Id || e.Id == entities[1].Id)
			.Should().NotBeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveManyAsync_UsingFilters_ShouldRemoveEntitiesFromDbSet_WhenEntitiesExist()
	{
		// Act
		await _repository.RemoveManyAsync(e => e.Id == 4 || e.Id == 5);

		// Assert
		_repository.Data.Where(e => e.Id == 4 || e.Id == 5).Should().BeNullOrEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveManyAsync_UsingFilters_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Act
		await _repository.RemoveManyAsync(e => e.Id == 6 || e.Id == 7);

		// Assert
		_context.Set<TestEntity>()
			.Where(e => e.Id == 6 || e.Id == 7)
			.Should().BeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task RemoveManyAsync_UsingFilters_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Act
		await _repository.RemoveManyAsync(e => e.Id == 4 || e.Id == 5, false);

		// Assert
		var removedEntities = _context.Set<TestEntity>()
			.Where(e => e.Id == 4 || e.Id == 5);
		removedEntities.Should().BeEmpty();

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

		// Act
		_repository.RemoveMany(e => entities.Select(e => e.Id).Contains(e.Id));

		// Assert
		_repository.Data.Should().NotContain(entities);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveMany_UsingFilters_ShouldSaveChangesToDatabase_WhenSaveChangesIsTrue()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 91, Name = "Entity 1" },
			new() { Id = 92, Name = "Entity 2" }
		};
		_repository.AddMany(entities);

		// Act
		_repository.RemoveMany(e => e.Id == entities[0].Id || e.Id == entities[1].Id, true);

		// Assert
		var removedEntities = _context.Set<TestEntity>()
			.Where(e => e.Id == entities[0].Id || e.Id == entities[1].Id);
		removedEntities.Should().BeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void RemoveMany_UsingFilters_ShouldNotSaveChangesToDatabase_WhenSaveChangesIsFalse()
	{
		// Arrange
		var entities = new List<TestEntity>
		{
			new() { Id = 41, Name = "Entity 1" },
			new() { Id = 42, Name = "Entity 2" }
		};
		_repository.AddMany(entities);

		// Act
		_repository.RemoveMany(e => e.Id == entities[0].Id || e.Id == entities[1].Id, false);

		// Assert
		var removedEntities = _context.Set<TestEntity>()
			.Where(e => e.Id == entities[0].Id || e.Id == entities[1].Id);
		removedEntities.Should().NotBeEmpty();

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task SaveAsync_ShouldSaveChangesToDatabase_WhenChangesExist()
	{
		// Arrange
		var entity = new TestEntity { Id = 1543, Name = "Entity" };
		await _repository.AddOneAsync(entity, false);

		// Act
		var result = await _repository.SaveAsync();

		// Assert
		result.Should().Be(1);
		var savedEntity = await _context.Set<TestEntity>().FindAsync(entity.Id);
		savedEntity.Should().Be(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public async Task SaveAsync_ShouldNotSaveChangesToDatabase_WhenNoChangesExist()
	{
		// Act
		var result = await _repository.SaveAsync();

		// Assert
		result.Should().Be(0);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void Save_ShouldSaveChangesToDatabase_WhenChangesExist()
	{
		// Arrange
		var entity = new TestEntity { Id = 198, Name = "Entity" };
		_repository.AddOne(entity, false);

		// Act
		var result = _repository.Save();

		// Assert
		result.Should().Be(1);
		var savedEntity = _context.Set<TestEntity>().Find(entity.Id);
		savedEntity.Should().Be(entity);

		_context.ChangeTracker.Clear();
	}

	[Fact]
	public void Save_ShouldNotSaveChangesToDatabase_WhenNoChangesExist()
	{
		// Act
		var result = _repository.Save();

		// Assert
		result.Should().Be(0);

		_context.ChangeTracker.Clear();
	}
}
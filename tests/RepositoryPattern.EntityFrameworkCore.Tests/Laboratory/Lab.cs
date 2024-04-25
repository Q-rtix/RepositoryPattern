using Microsoft.EntityFrameworkCore;
using RepositoryPattern.EntityFrameworkCore.Repositories;

namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

internal static class Lab
{
	public static IEnumerable<TestEntity> TestEntities =>
	[
		new TestEntity { Id = 1, Name = "Test 1", RelatedTestEntities = [RelatedTestEntities[0]] },
		new TestEntity { Id = 2, Name = "Test 2", RelatedTestEntities = [RelatedTestEntities[1], RelatedTestEntities[2]]},
		new TestEntity { Id = 3, Name = "Test 3", RelatedTestEntities = [RelatedTestEntities[3], RelatedTestEntities[4], RelatedTestEntities[5]]},
		new TestEntity { Id = 4, Name = "Test 4"},
		new TestEntity { Id = 5, Name = "Test 5"},
		new TestEntity { Id = 6, Name = "Test 6"},
		new TestEntity { Id = 7, Name = "Test 7"},
		new TestEntity { Id = 8, Name = "Test 8"},
		new TestEntity { Id = 9, Name = "Test 9"},
		new TestEntity { Id = 10, Name = "Test 10"}
	];

	public static List<RelatedTestEntity> RelatedTestEntities =>
	[
		new RelatedTestEntity { Id = 1, Name = "Related 1", TestEntityId = 1 },
		new RelatedTestEntity { Id = 2, Name = "Related 2", TestEntityId = 2 },
		new RelatedTestEntity { Id = 3, Name = "Related 3", TestEntityId = 2 },
		new RelatedTestEntity { Id = 4, Name = "Related 4", TestEntityId = 3 },
		new RelatedTestEntity { Id = 5, Name = "Related 5", TestEntityId = 3 },
		new RelatedTestEntity { Id = 6, Name = "Related 6", TestEntityId = 3 }
	];
}
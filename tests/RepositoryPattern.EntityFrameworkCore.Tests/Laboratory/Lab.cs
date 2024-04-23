using Microsoft.EntityFrameworkCore;
using RepositoryPattern.EntityFrameworkCore.Repositories;

namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

internal static class Lab
{
	public static Repository<TestEntity> CreateRepositoryInstance()
	{
		var options = new DbContextOptionsBuilder<TestDbContext>()
			.UseInMemoryDatabase("TestDB")
			.Options;

		var context = new TestDbContext(options);
	}
}
using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

public class TestDbContext : DbContext
{
	public DbSet<TestEntity> TestEntities { get; set; }

	public TestDbContext(DbContextOptions<TestDbContext> options)
	: base(options) { }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.Entity<TestEntity>()
			.HasKey(e => e.Id);

		builder.Entity<TestEntity>()
			.Property(e => e.Id)
			.IsRequired();

		builder.Entity<TestEntity>()
			.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(120);

		builder.Entity<TestEntity>()
			.Property(e => e.Description)
			.HasMaxLength(250);
		
		base.OnModelCreating(builder);
	}
}
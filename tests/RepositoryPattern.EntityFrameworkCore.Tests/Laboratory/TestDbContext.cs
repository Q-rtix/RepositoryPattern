using Microsoft.EntityFrameworkCore;

namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

public class TestDbContext : DbContext
{
	public DbSet<TestEntity> TestEntities { get; set; }
	public DbSet<RelatedTestEntity> RelatedTestEntities { get; set; }
	public DbSet<InsideRelatedTestEntity> InsideRelatedTestEntities { get; set; }

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

		builder.Entity<TestEntity>()
			.HasMany<RelatedTestEntity>()
			.WithOne(r => r.TestEntity)
			.HasForeignKey(r => r.TestEntityId);
		
		builder.Entity<RelatedTestEntity>()
			.HasKey(e => e.Id);

		builder.Entity<RelatedTestEntity>()
			.HasOne<InsideRelatedTestEntity>()
			.WithMany(ir => ir.RelatedTestEntities)
			.HasForeignKey(r => r.InsideRelatedTestEntityId);

		builder.Entity<InsideRelatedTestEntity>()
			.HasKey(e => e.Id);
		
		base.OnModelCreating(builder);
	}
}
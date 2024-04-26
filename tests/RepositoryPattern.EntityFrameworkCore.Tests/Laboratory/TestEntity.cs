namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

public class TestEntity
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }

	public virtual ICollection<RelatedTestEntity> RelatedTestEntities { get; set; } = new HashSet<RelatedTestEntity>();

	public override bool Equals(object? obj)
		=> obj is TestEntity entity && Equals(entity);

	protected bool Equals(TestEntity other)
	{
		return Id == other.Id && Name == other.Name && Description == other.Description;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Name, Description);
	}
}
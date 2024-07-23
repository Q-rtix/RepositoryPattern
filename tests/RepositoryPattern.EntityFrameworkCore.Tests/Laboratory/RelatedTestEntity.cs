namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

public class RelatedTestEntity
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }

	public int? TestEntityId { get; set; }
	public TestEntity? TestEntity { get; set; }
	
	public int? InsideRelatedTestEntityId { get; set; }
	public InsideRelatedTestEntity? InsideRelatedTestEntity { get; set; }

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
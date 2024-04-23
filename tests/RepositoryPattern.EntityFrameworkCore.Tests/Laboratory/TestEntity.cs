namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

public class TestEntity
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
}
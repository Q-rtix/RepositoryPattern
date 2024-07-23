namespace RepositoryPattern.EntityFrameworkCore.Tests.Laboratory;

public class InsideRelatedTestEntity
{
	public required int Id { get; set; }
	
	public ICollection<RelatedTestEntity> RelatedTestEntities { get; set; } = new HashSet<RelatedTestEntity>();
}
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Abstractions.Builder;

namespace RepositoryPattern.Abstractions.Tests.Builder;

public class UnitOfWorkOptionsTests
{
	[Fact]
	public void CreateUnitOfWorkOptions_ShouldBeSuccessful()
	{
		// Act
		var options = new UnitOfWorkOptions(typeof(Lab.UnitOfWork));

		// Assert
		options.Should().NotBeNull();
		options.ImplementationType.Should().Be(typeof(Lab.UnitOfWork));
		options.Lifetime.Should().Be(ServiceLifetime.Scoped);
	}
	
	[Fact]
	public void CreateUnitOfWorkOptions_ShouldThrowArgumentException()
	{
		// Act
		Action act = () => new UnitOfWorkOptions(typeof(ArgumentException));

		// Assert
		act.Should().Throw<ArgumentException>();
	}
	
	[Fact]
	public void CreateUnitOfWorkOptions_ShouldBeSuccessful_WithLifetime()
	{
		// Act
		var options = new UnitOfWorkOptions(typeof(Lab.UnitOfWork)).UseLifetime(ServiceLifetime.Singleton);

		// Assert
		options.Should().NotBeNull();
		options.ImplementationType.Should().Be(typeof(Lab.UnitOfWork));
		options.Lifetime.Should().Be(ServiceLifetime.Singleton);
	}
}
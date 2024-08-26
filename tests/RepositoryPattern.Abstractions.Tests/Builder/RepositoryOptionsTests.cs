using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RepositoryPattern.Abstractions.Builder;
using RepositoryPattern.Abstractions.Tests.Lab;

namespace RepositoryPattern.Abstractions.Tests.Builder;

public class RepositoryOptionsTests
{
	[Fact]
	public void CreateRepositoryOptions_ShouldBeSuccessful()
	{
		// Act
		var options = new RepositoryOptions(typeof(Repository<>));
		

		// Assert
		options.Should().NotBeNull();
		options.ImplementationType.Should().Be(typeof(Repository<>));
		options.Lifetime.Should().Be(ServiceLifetime.Scoped);
	}
	
	[Fact]
	public void CreateRepositoryOptions_ShouldThrowArgumentException()
	{
		// Act
		Action act = () => new RepositoryOptions(typeof(NotImplementedRepo));

		// Assert
		act.Should().Throw<ArgumentException>();
	}
	
	[Fact]
	public void CreateRepositoryOptions_ShouldBeSuccessful_WithLifetime()
	{
		// Act
		var options = new RepositoryOptions(typeof(Repository<>)).UseLifetime(ServiceLifetime.Singleton);

		// Assert
		options.Should().NotBeNull();
		options.ImplementationType.Should().Be(typeof(Repository<>));
		options.Lifetime.Should().Be(ServiceLifetime.Singleton);
	}
}
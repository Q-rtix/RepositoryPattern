# Repository Pattern Abstractions

![NuGet Version](https://img.shields.io/nuget/v/Qrtix.RepositoryPattern.Abstractions?style=flat&logo=nuget)
![NuGet Downloads](https://img.shields.io/nuget/dt/Qrtix.RepositoryPattern.Abstractions?style=flat&logo=nuget)
![GitHub Repo stars](https://img.shields.io/github/stars/Q-rtix/RepositoryPattern?style=flat&logo=github)

A library for implementing generic repositories and unit of work with Entity Framework Core.
This implementation uses a single instance of the DbContext for all repositories to avoid concurrency issues.

Consult the online [documentation](https://q-rtix.github.io/RepositoryPattern/) for more details.

## Table of Contents

- [Installation](#installation)
- [Creating the DbContext](#creating-the-dbcontext)
- [Injecting the Repository Pattern](#injecting-the-efcorerepositorypattern-services)
- [Creating the Service](#creating-the-service)
- [IUnitOfWork methods](#unitofwork)
- [IRepository methods](#repository-methods)

## Installation

Using the NuGet package manager console within Visual Studio run the following command:

```
Install-Package Ortix.RepositoryPattern.Abstractions
```

Or using the .NET Core CLI from a terminal window:

```
dotnet add package Qrtix.RepositoryPattern.Abstractions
```

## Creating the DbContext

Create your DbContext inheriting from `Microsoft.EntityFrameworkCore.DbContext`:

```csharp
public class MyDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}
}
```

Configure your DbContext in the `Program` class:

```csharp
builder.Services.AddDbContext<DbContext, SuinQShopModuleDbContext>(opt =>
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
    });
```

## Creating the Repository Implementation

Create your repository implementation inheriting from `IRepository<TEntity>`:

```csharp
public class ProductRepository : IRepository<TEntity> where TEntity : class
{
	private readonly DbSet<TEntity> _dbSet;
	private readonly DbContext _context;
	
    public IQueryable<TEntity> Data => _dbSet;


	public async Task<IQueryable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>>? filters = null,
		bool disableTracking = true,
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
		CancellationToken cancellationToken = default)
	{
		// implementation
	}

	public IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>>? filters = null,
		bool disableTracking = false,
		IEnumerable<Expression<Func<TEntity, object>>>? includes = null,
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
		{
            // implementation
        }

	// others implementation 
}
```

## Creating the Unit Of Work implementation

Create your unit of work implementation inheriting from `IUnitOfWork`:

```csharp
class UnitOfWork : IUnitOfWork
{
	private readonly DbContext _context;
	private IDbContextTransaction? _transaction;
	private readonly Dictionary<string, object> _repositories;

	public UnitOfWork(DbContext context)
	{
		_context = context;
		_repositories = new Dictionary<string, object>();
	}

	public void Dispose()
	{
		// implementation
	}


	public async ValueTask DisposeAsync()
	{
		// implementation
	}

	public IRepository<TEntity> Repository<TEntity>() where TEntity : class
	{
		// implementation
	}

	// others implementations
```

## Injecting the RepositoryPattern's Services

Add the `RepositoryPattern` services to the Service Collection:

```csharp
builder.Services.AddRepositoryPattern(options => {
    options.UseRepositoryImplementation(typeof(Repository<>)
    	.UseUnitOfWorkImplementation<UnitOfWork>();
});
```

The default scope for injected services is scoped. If you want to change it, refer to the next example:

```csharp
builder.Services.AddRepositoryPattern(options => {
    options.UseRepositoryImplementation(typeof(Repository<>)
    	.UseUnitOfWorkImplementation<UnitOfWork>();
}, ServiceLifeTime.Transient);
```
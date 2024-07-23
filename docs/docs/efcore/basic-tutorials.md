# Basic Tutorials

This guide provides examples of basic usage for the repository pattern implementation with Entity Framework Core.

You are able to use the methods inherited from the `IQueryable<TEntity>` interface and can be used directly with 
the repository:
- **Filtering Data**: Use methods like `Where` for filtering.
- **Ordering Data**: Use methods like `OrderBy` for sorting.
- **Projecting Data**: Use methods like `Select` for projection.

These methods provide powerful querying capabilities and can be used in conjunction with other repository methods for
more complex data operations.

>[!Important]
> To save changes into the database, use the `SaveChanges` method or the async version from the UnitOfWork.

## Table of Contents

- [Retrieving Data](#retrieving-data)
- [Saving Data](#saving-data)
- [Updating Data](#updating-data)
- [Deleting Data](#deleting-data)

## Retrieving Data

### Example: Retrieving a Single Entity

You can use `GetOne` and `FirstOrDefault` or the async versions to retrieve a single entity from the repository.

```csharp
public async Task<Product> GetProductByIdAsync(int productId)
{
	// You can also use synchronous version of GetOne
    Product product = await _unitOfWork.Repository<Product>().GetOneAsync(p => p.Id == productId);

    // You can also use synchronous version of FirstOrDefault
	product = await _unitOfWork.Repository<Product>().FirstOrDefaultAsync(p => p.Id == productId);

    if (product == null)
    {
        throw new Exception($"Product with Id {productId} not found.");
    }

    return product;
}
```

### Example: Retrieving Multiple Entities

You can use `GetMany`, `GetManyAsync` to retrieve multiple entities from the repository or use the IQueryable methods
like `Where`, `OrderBy`, `Select` to filter, sort, and project data directly from the repository.

```csharp
public async Task<List<Product>> GetProductsByCategoryAsync(string category)
{
	// You can also use synchronous version of GetMany
	List<Product> products = await _unitOfWork.Repository<Product>().GetManyAsync(p => p.Category == category);
    
    // Also you can use the IQueryable methods like Where, OrderBy, Select directly on the repository
	products = await _unitOfWork.Repository<Product>().Where(p => p.Category == category).ToListAsync();

    return products;
}
```

### Example: Using the 'includes' parameter in GetMany and GetOne methods

You can use the `includes` parameter to include related entities in the query.

```csharp
public async Task<List<Product>> GetProductsWithCategoryAsync()
{
	List<Product> products = await _unitOfWork.Repository<Product>().GetManyAsync(includes: p => p.Category);
	return products;
}
```

If you want to include multiple related entities, you can use the `includes` parameter with multiple expressions.

```csharp
public async Task<List<Product>> GetProductsWithCategoryAndSupplierAsync()
{
    var includes = new Expression<Func<Product, object>>[]
	{
		p => p.Category,
		p => p.Suppliers
	};
	List<Product> products = await _unitOfWork.Repository<Product>().GetManyAsync(includes: includes);
	return products;
}
```

Now you want access to the contact info of the supplier, you can use the `Then` method if the suppliers is a collection to provide the navigation property.

```csharp
public async Task<List<Product>> GetProductsWithCategoryAndSupplierContactInfoAsync()
{
	var includes = new Expression<Func<Product, object>>[]
	{
		p => p.Category,
		p => p.Suppliers.Then(s => s.ContactInfo)
	};
	List<Product> products = await _unitOfWork.Repository<Product>().GetManyAsync(includes: includes);
	return products;
}
```

## Saving Data

### Example: Saving an Entity

You can use `AddOne` or the async versions to save an entity to the repository.

```csharp
public async Task AddProductAsync(Product product)
{
	await _unitOfWork.Repository<Product>().AddOneAsync(product);
    await _unitOfWork.SaveAsync();
}
```

### Example: Saving Multiple Entities

You can use `AddMany` or the async versions to save multiple entities to the repository.

```csharp
public async Task AddProductsAsync(List<Product> products)
{
	await _unitOfWork.Repository<Product>().AddManyAsync(products);
	await _unitOfWork.SaveAsync();
}
```

## Updating Data

### Example: Updating an Entity

You can use `UpdateOne` or the async versions to update an entity in the repository.

```csharp
public async Task UpdateProductAsync(Product product)
{
	await _unitOfWork.Repository<Product>().UpdateOneAsync(product);
    await _unitOfWork.SaveAsync();
}
```

### Example: Updating Multiple Entities

You can use `UpdateMany` or the async versions to update multiple entities in the repository.

```csharp
public async Task UpdateProductsAsync(List<Product> products)
{
	await _unitOfWork.Repository<Product>().UpdateManyAsync(products);
	await _unitOfWork.SaveAsync();
}
```

## Deleting Data

### Example: Deleting an Entity

You can use `RemoveOne` or the async versions to delete an entity from the repository.

```csharp
public async Task RemoveProductAsync(Product product)
{
	await _unitOfWork.Repository<Product>().RemoveOneAsync(product);
	await _unitOfWork.SaveAsync();
}
```

### Example: Deleting Multiple Entities

You can use `RemoveMany` or the async versions to delete multiple entities from the repository.

```csharp
public async Task RemoveProductsAsync(List<Product> products)
{
	await _unitOfWork.Repository<Product>().RemoveManyAsync(products);
	await _unitOfWork.SaveAsync();
}
```
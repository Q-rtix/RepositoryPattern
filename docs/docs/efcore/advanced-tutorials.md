# Advanced Usage

This section delves into advanced scenarios for utilizing the `UnitOfWork` class. Here, you will learn how to leverage
the full potential of the `UnitOfWork` pattern, including comprehensive
transaction management, the use of save points, and handling multiple repositories.
By mastering these techniques, you can ensure robust and efficient data management in your applications, minimizing
risks and optimizing performance.

## Advanced Transaction Management

Efficient transaction management is crucial for maintaining data integrity and consistency. The `UnitOfWork` class
provides robust support for starting, committing, and rolling back transactions. This ensures that a series of
operations can either be completed successfully as a unit or reverted entirely if any operation fails.

Here's how you can manage transactions effectively:

### Starting a Transaction

To start a transaction, use the `BeginTransaction` method. This method opens a connection and starts a new transaction.

```csharp
public void StartTransactionExample()
{
    unitOfWork.BeginTransaction();
    try
    {
        // Perform data operations
        unitOfWork.Repository<Product>().Add(new Product { Name = "Product1" });
        unitOfWork.Save();

        // Commit the transaction
        unitOfWork.Commit();
    }
    catch (Exception)
    {
        // Roll back the transaction in case of an error
        unitOfWork.RollBack();
        throw;
    }
}
```

### Asynchronous Transaction

For asynchronous operations, use BeginTransactionAsync along with async/await patterns:

```csharp
public async Task StartTransactionAsyncExample()
{
    await unitOfWork.BeginTransactionAsync();
    try
    {
        // Perform data operations
        await unitOfWork.Repository<Product>().AddAsync(new Product { Name = "Product1" });
        await unitOfWork.SaveAsync();

        // Commit the transaction
        await unitOfWork.CommitAsync();
    }
    catch (Exception)
    {
        // Roll back the transaction in case of an error
        await unitOfWork.RollBackAsync();
        throw;
    }
}
```

## Using Save Points

Save points offer finer control within transactions, allowing partial rollbacks without affecting the entire
transaction. This feature is particularly useful for complex business logic where certain steps may need to be retried
or reversed without discarding all preceding successful operations.

### Creating and Rolling Back to Save Points:

```csharp
public void SavePointExample()
{
    unitOfWork.BeginTransaction();
    try
    {
        unitOfWork.Repository<Product>().Add(new Product { Name = "Product1" });
        unitOfWork.Save();
        
        unitOfWork.CreateSavePoint("SavePoint1");

        unitOfWork.Repository<Product>().Add(new Product { Name = "Product2" });
        unitOfWork.Save();

        unitOfWork.RollBackToSavePoint("SavePoint1");

        unitOfWork.Commit();
    }
    catch (Exception)
    {
        unitOfWork.RollBack();
        throw;
    }
}
```

## Handling Multiple Repositories

In scenarios where multiple entities need to be managed simultaneously, the `UnitOfWork` class facilitates the
coordination of operations across different repositories within the same transaction. This ensures consistency and
atomicity of related data changes.

```csharp
public void MultipleRepositoriesExample()
{
    unitOfWork.BeginTransaction();
    try
    {
        var productRepository = unitOfWork.Repository<Product>();
        var orderRepository = unitOfWork.Repository<Order>();

        productRepository.Add(new Product { Name = "Product1" });
        orderRepository.Add(new Order { OrderNumber = "Order1" });
        
        unitOfWork.Save();

        unitOfWork.Commit();
    }
    catch (Exception)
    {
        unitOfWork.RollBack();
        throw;
    }
}
```
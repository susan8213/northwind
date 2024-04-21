using Dapper;
using Microsoft.Data.SqlClient;
using Northwind.Models;

namespace Northwind.Access;

public class SqlOrderRepository : IOrderRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqlCustomerRepository> _logger;

    public SqlOrderRepository(ILogger<SqlCustomerRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["ConnectionString"];
    }


    public async Task<Order> GetAsync(int id)
    {
        Dictionary<int,Order> orderDictionary = new Dictionary<int, Order>();
        using (var connection = new SqlConnection(_connectionString)) 
        {    

            var sql = @"SELECT O.[OrderID], O.[OrderDate], O.[RequiredDate], O.[ShippedDate], 
                OD.[UnitPrice], OD.[Quantity], OD.[Discount], P.[ProductID], P.[ProductName]
            FROM [Orders] O
            INNER JOIN [Order Details] OD ON O.OrderID = OD.OrderID
            INNER JOIN [Products] P ON OD.ProductID = P.ProductID
            WHERE O.[OrderID] = @OrderId";     
            
            // Use the Query method to execute the query and return a list of objects  
            var orders = await connection.QueryAsync<Order, OrderDetail, Product, Order>(sql, 
                (order, orderDetail, product) => {
                    if (!orderDictionary.TryGetValue(order.OrderID, out Order orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.OrderDetails = new List<OrderDetail>();
                        orderDictionary.Add(orderEntry.OrderID, orderEntry);
                    }
                    orderDetail.Product = product;
                    orderEntry.OrderDetails.Add(orderDetail);
                    return orderEntry;
                }, 
                param: new {OrderId = id},
                splitOn: "UnitPrice, ProductID"
            );

            return orders.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
    {
        Dictionary<int,Order> orderDictionary = new Dictionary<int, Order>();
        using (var connection = new SqlConnection(_connectionString)) 
        {    

            var sql = @"SELECT O.[OrderID], O.[OrderDate], O.[RequiredDate], O.[ShippedDate], 
                OD.[UnitPrice], OD.[Quantity], OD.[Discount], P.[ProductID], P.[ProductName]
            FROM [Orders] O
            INNER JOIN [Order Details] OD ON O.OrderID = OD.OrderID
            INNER JOIN [Products] P ON OD.ProductID = P.ProductID
            WHERE [CustomerID] = @CustomerId";     
            
            // Use the Query method to execute the query and return a list of objects  
            var orders = await connection.QueryAsync<Order, OrderDetail, Product, Order>(sql, 
                (order, orderDetail, product) => {
                    if (!orderDictionary.TryGetValue(order.OrderID, out Order orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.OrderDetails = new List<OrderDetail>();
                        orderDictionary.Add(orderEntry.OrderID, orderEntry);
                    }
                    orderDetail.Product = product;
                    orderEntry.OrderDetails.Add(orderDetail);
                    return orderEntry;
                }, 
                param: new {CustomerId = customerId},
                splitOn: "UnitPrice, ProductID"
            );

            return orders;
        }
    }

    public async Task<int> CreateAsync(Order order)
    {
        using (var connection = new SqlConnection(_connectionString))
        { 
            connection.Open();

            using(SqlTransaction tran = connection.BeginTransaction()) 
            {
                try
                {
                    var sqlCreateOrder = @"INSERT INTO [Orders] (CustomerID, OrderDate, RequiredDate, ShippedDate) VALUES (@CustomerID, @OrderDate, @RequiredDate, @ShippedDate)
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    var sqlCreateDetail = @"INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount) VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";
                    var sqlGetProduct = @"SELECT [ProductID], [UnitsInStock] FROM [Products] WHERE [ProductID]=@ProductID";
                    var sqlUpdateProductStock = @"UPDATE [Products] SET [UnitsInStock]=@UnitsInStock WHERE [ProductID] = @ProductID";

                    // pass the transaction along to the Query, Execute, or the related Async methods.
                    int orderId = await connection.QuerySingleAsync<int>(sqlCreateOrder, order, tran);
                    order.OrderID = orderId;
                    var orderDetails = new List<object>();
                    var productUpdates = new List<object>();
                    foreach(var detail in order.OrderDetails)
                    {
                        orderDetails.Add(new {
                            OrderID = orderId, 
                            ProductID=detail.Product.ProductID, 
                            UnitPrice=detail.UnitPrice,
                            Quantity=detail.Quantity,
                            Discount=detail.Discount
                        });

                        var product = await connection.QuerySingleAsync<Product>(sqlGetProduct, new {ProductID=detail.Product.ProductID}, tran);
                        productUpdates.Add(new {
                            ProductID=product.ProductID,
                            UnitsInStock=product.UnitsInStock - detail.Quantity
                        });

                    }
                    await connection.ExecuteAsync(sqlCreateDetail, orderDetails, tran);
                    await connection.ExecuteAsync(sqlUpdateProductStock, productUpdates, tran);

                    // if it was successful, commit the transaction
                    tran.Commit();
                    return orderId;
                }
                catch(Exception ex)
                {
                    // roll the transaction back
                    tran.Rollback();

                    // handle the error however you need to.
                    _logger.LogError($"Error occured while creating order of customer [{order.CustomerID}] in DB.");
                    throw;
                }
            }
        }
    }

    // Should be soft deleted, just for demo of CRUD and not changing db schema
    public async Task DeleteAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        { 
            connection.Open();

            using(SqlTransaction tran = connection.BeginTransaction()) 
            {
                try
                {
                    var sqlDeleteOrder = @"DELETE FROM [Orders] WHERE OrderID=@OrderID";
                    var sqlGetProductsFromOrderDetails = @"SELECT P.[ProductID], (P.[UnitsInStock] + OD.[Quantity]) AS [UnitsInStock] FROM [Order Details] OD INNER JOIN [Products] P ON OD.ProductID = P.ProductID WHERE OrderID=@OrderID";
                    var sqlDeleteDetail = @"DELETE FROM [Order Details] WHERE OrderID=@OrderID";
                    var sqlUpdateProductStock = @"UPDATE [Products] SET [UnitsInStock]=@UnitsInStock WHERE [ProductID] = @ProductID";

                    // pass the transaction along to the Query, Execute, or the related Async methods.
                    var productUpdates = await connection.QueryAsync<Product>(sqlGetProductsFromOrderDetails, new {OrderID=id}, tran);
                    await connection.ExecuteAsync(sqlDeleteDetail, new {OrderID=id}, tran);
                    await connection.ExecuteAsync(sqlDeleteOrder, new {OrderID=id}, tran);
                    await connection.ExecuteAsync(sqlUpdateProductStock, productUpdates, tran);

                    // if it was successful, commit the transaction
                    tran.Commit();
                }
                catch(Exception ex)
                {
                    // roll the transaction back
                    tran.Rollback();

                    // handle the error however you need to.
                    _logger.LogError($"Error occured while deleting order [{id}] in DB.");
                    throw;
                }
            }
        }
    }

    public async Task UpdateAsync(Order order)
    {
        using (var connection = new SqlConnection(_connectionString))
        { 
            connection.Open();

            using(SqlTransaction tran = connection.BeginTransaction()) 
            {
                try
                {
                    var sql = @"UPDATE [Orders] SET RequiredDate=@RequiredDate, ShippedDate=@ShippedDate WHERE OrderID=@OrderID";

                    // pass the transaction along to the Query, Execute, or the related Async methods.
                    await connection.ExecuteAsync(sql, order, tran);

                    // if it was successful, commit the transaction
                    tran.Commit();
                }
                catch(Exception ex)
                {
                    // roll the transaction back
                    tran.Rollback();

                    // handle the error however you need to.
                    _logger.LogError($"Error occured while updating order [{order.OrderID}] in DB.");
                    throw;
                }
            }
        }
    }
}
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

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId)
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
}
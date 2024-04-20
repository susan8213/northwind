using Dapper;
using Microsoft.Data.SqlClient;
using Northwind.Models;

namespace Northwind.Access;

public class SqlProductRepository : IProductRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqlProductRepository> _logger;

    public SqlProductRepository(ILogger<SqlProductRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["ConnectionString"];
    }

    public async Task<Product> GetAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString)) 
        {
            var sql = @"SELECT [ProductID], [ProductName], [QuantityPerUnit], [UnitPrice], [UnitsInStock] FROM [Products] WHERE [ProductID]=@ProductID AND [Discontinued]=0";
            var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new {ProductID=id});
            return product;
        }
    }
}
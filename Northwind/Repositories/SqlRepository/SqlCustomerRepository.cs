using Dapper;
using Microsoft.Data.SqlClient;
using Northwind.Models;

namespace Northwind.Access;

public class SqlCustomerRepository : ICustomerRepository{

    private readonly string _connectionString;
    private readonly ILogger<SqlCustomerRepository> _logger;

    public SqlCustomerRepository(ILogger<SqlCustomerRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration["ConnectionString"];
    }

    public async Task<Customer> Get(string customerId) {
        // Connect to the database 
        using (var connection = new SqlConnection(_connectionString)) 
        {    
            var sql = "SELECT [CustomerID], [CompanyName], [ContactName], [City], [Region], [Country] FROM [Customers] WHERE [CustomerID] = @CustomerId";     
            
            // Use the Query method to execute the query and return a list of objects  
            var customer = await connection.QuerySingleOrDefaultAsync<Customer>(sql, new {CustomerId = customerId}); 
            return customer;
        }
    }
}
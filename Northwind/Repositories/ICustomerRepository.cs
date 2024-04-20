using Northwind.Models;

namespace Northwind.Access;

public interface ICustomerRepository {
    Task<Customer> Get(string customerId);
}
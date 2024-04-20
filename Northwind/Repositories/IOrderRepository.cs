using Northwind.Models;

namespace Northwind.Access;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId);
}
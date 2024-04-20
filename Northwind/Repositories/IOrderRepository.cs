using Northwind.Models;

namespace Northwind.Access;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);
    Task<Order> GetAsync(int id);
    Task<int> CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int id);
}
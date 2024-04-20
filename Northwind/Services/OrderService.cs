using Microsoft.AspNetCore.Http.HttpResults;
using Northwind.Access;
using Northwind.Exceptions;
using Northwind.Models;

namespace Northwind.Services;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _orderRepository;

    public OrderService(ILogger<OrderService> logger, IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }
    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId)
    {
       var result = await _orderRepository.GetOrdersByCustomerId(customerId);
       if (result.Count() == 0) throw new NotFoundException();
       return result;
    }
}
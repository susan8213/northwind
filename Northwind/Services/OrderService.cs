using Microsoft.AspNetCore.Http.HttpResults;
using Northwind.Access;
using Northwind.Exceptions;
using Northwind.Models;

namespace Northwind.Services;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderService(ILogger<OrderService> logger, IOrderRepository orderRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Order> GetOrderAsync(int id) {
        var result = await _orderRepository.GetAsync(id);
        if (result == null) throw new NotFoundException();
        return result;
    }
    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(string customerId)
    {
       var result = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
       if (result.Count() == 0) throw new NotFoundException();
       return result;
    }

    public async Task<int> CreateAsync(Order order)
    {
        var customer = await _customerRepository.Get(order.CustomerID);
        if (customer == null) throw new NotFoundException($"Customer [{order.CustomerID}] does not exist.");
        foreach(var orderDetail in order.OrderDetails)
        {
            Product product = await _productRepository.GetAsync(orderDetail.Product.ProductID);
            if (product == null) 
                throw new NotFoundException($"Product [{orderDetail.Product.ProductID}] does not exist.");

            if (orderDetail.Quantity > product.UnitsInStock) 
                throw new ValidationException($"Product [{product.ProductID}] {product.ProductName} is out of stock.");
            
            orderDetail.UnitPrice = product.UnitPrice;
        }

        
        return await _orderRepository.CreateAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        Order originalOrder = await GetOrderAsync(order.OrderID);
        if (originalOrder == null) throw new NotFoundException($"Order [{order.OrderID}] does not exist.");
        if (originalOrder.ShippedDate != null) throw new ValidationException($"Order [{order.OrderID}] cannot be edited, because of it is already shipped.");

        await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteAsync(int orderId)
    {
        await _orderRepository.DeleteAsync(orderId);
    }
}
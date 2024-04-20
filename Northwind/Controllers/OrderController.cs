using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Northwind.DTO;
using Northwind.Models;
using Northwind.Services;

namespace Northwind.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly OrderService _orderService;

    public OrderController(ILogger<OrderController> logger, OrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponse>> Get([FromRoute] int orderId)
    {
        var result = await _orderService.GetOrderAsync(orderId);
        return new OkObjectResult(new OrderResponse(result));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOrderRequest orderRequest)
    {
        List<OrderDetail> orderDetails = new List<OrderDetail>();
        foreach(var orderDetailRequest in orderRequest.OrderDetails)
        {
            orderDetails.Add(new OrderDetail
                {
                    Product = new Product { ProductID = orderDetailRequest.ProductID},
                    Quantity = orderDetailRequest.Quantity,
                    Discount = orderDetailRequest.Discount
                }
            );
        }

        Order order = new Order 
        {
            CustomerID = orderRequest.CustomerID,
            OrderDate = DateTime.UtcNow,
            RequiredDate = orderRequest.RequiredDate,
            ShippedDate = orderRequest.ShippedDate,
            OrderDetails = orderDetails
        };

        var result = await _orderService.CreateAsync(order);
        return CreatedAtAction(nameof(Create), new { id = order.OrderID }, order);

    }

    [HttpPut("{orderId}")]
    public async Task<ActionResult<OrderResponse>> Update([FromRoute] int orderId, [FromBody] UpdateOrderRequest orderRequest)
    {


        Order order = new Order 
        {
            OrderID = orderId,
            RequiredDate = orderRequest.RequiredDate,
            ShippedDate = orderRequest.ShippedDate
        };

        await _orderService.UpdateAsync(order);
        return NoContent();
    }

    [HttpDelete("{orderId}")]
    public async Task<ActionResult> Delete([FromRoute] int orderId)
    {
        await _orderService.DeleteAsync(orderId);
        return NoContent();
    }


}
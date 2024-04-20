using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Northwind.Access;
using Northwind.DTO;
using Northwind.Exceptions;
using Northwind.Models;
using Northwind.Services;

namespace Northwind.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{

    private readonly ILogger<CustomerController> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly OrderService _orderService;

    public CustomerController(ILogger<CustomerController> logger, ICustomerRepository customerRepository, OrderService orderService)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _orderService = orderService;
    }

    [HttpGet("{customerId}")]
    public async Task<ActionResult<Customer>> GetCustomer([FromRoute][Required] string customerId)
    {
        var result = await _customerRepository.Get(customerId);
        if (result == null) throw new NotFoundException();
        
        return new OkObjectResult(result);
    }

    [HttpGet("{customerId}/orders")]
    public async Task<ActionResult<CustomerOrderResponse>> GetCustomerOrders([FromRoute][Required] string customerId)
    {
        var result = await _orderService.GetOrdersByCustomerId(customerId);
        var response = new CustomerOrderResponse(result);
        return new OkObjectResult(response);
    }
}

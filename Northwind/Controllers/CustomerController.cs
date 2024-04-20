using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Northwind.Access;
using Northwind.Models;

namespace Northwind.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{

    private readonly ILogger<CustomerController> _logger;
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ILogger<CustomerController> logger, ICustomerRepository customerRepository)
    {
        _logger = logger;
        _customerRepository = customerRepository;
    }

    [HttpGet("{customerId}")]
    public async Task<ActionResult<Customer>> GetCustomer([FromRoute][Required] string customerId)
    {
        return await _customerRepository.Get(customerId);
    }
}

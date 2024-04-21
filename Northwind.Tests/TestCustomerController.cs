using Northwind.Access;
using Northwind.Models;
using Northwind.Controllers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Northwind.Exceptions;
using Northwind.Services;

namespace Northwind.Tests;

[TestClass]
public class TestCustomerController
{

    private ICustomerRepository _customerRepository;
    private ILogger<CustomerController> _logger;
    private OrderService _orderService;
    private ILogger<OrderService> _loggerOrderService;
    private IOrderRepository _orderRepository;
    private IProductRepository _productRepository;

    [TestInitialize]
    public void SetUp()
    {
        _customerRepository =  Substitute.For<ICustomerRepository>();
        _logger = Substitute.For<ILogger<CustomerController>>();

        _orderRepository =  Substitute.For<IOrderRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _orderService = new OrderService(_loggerOrderService, _orderRepository, _productRepository, _customerRepository);
    }

    [TestMethod]
    public async Task Test_GetCustomer_ExistingCustomerId_ShouldReturnCorrectCustomer()
    {
        // Arrange
        string expectedCustomerId = "test"; 
        Customer customer = new Customer {CustomerID=expectedCustomerId};
        _customerRepository.Get(Arg.Is(expectedCustomerId)).Returns(customer);

        // Act
        var contoller = new CustomerController(_logger, _customerRepository, _orderService);
        var result = await contoller.GetCustomer(expectedCustomerId);

        // Assert
        var output = (result.Result as OkObjectResult).Value as Customer;
        Assert.AreEqual(expectedCustomerId, output.CustomerID);
    }

    [TestMethod]
    public void Test_GetCustomer_NonexistingCustomerId_ShouldReturnNotFound()
    {
        // Arrange
        string expectedCustomerId = "test"; 
        Customer customer = null;
        _customerRepository.Get(Arg.Is(expectedCustomerId)).Returns(customer);

        // Act
        var contoller = new CustomerController(_logger, _customerRepository, _orderService);
        try
        {
            var result = contoller.GetCustomer(expectedCustomerId).Result;
            // Assert
            Assert.Fail("An exception should have been thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex.InnerException, typeof(NotFoundException));
        }
        
    }
}
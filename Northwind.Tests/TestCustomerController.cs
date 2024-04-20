using Northwind.Access;
using Northwind.Models;
using Northwind.Controllers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Northwind.Exceptions;

namespace Northwind.Tests;

[TestClass]
public class TestCustomerController
{

    private ICustomerRepository _customerRepository;
    private ILogger<CustomerController> _logger;

    [TestInitialize]
    public void SetUp()
    {
        _customerRepository =  Substitute.For<ICustomerRepository>();
        _logger = Substitute.For<ILogger<CustomerController>>();
    }

    [TestMethod]
    public async Task Test_GetCustomer_ExistingCustomerId_ShouldReturnCorrectCustomer()
    {
        // Arrange
        string expectedCustomerId = "test"; 
        Customer customer = new Customer {CustomerID=expectedCustomerId};
        _customerRepository.Get(Arg.Is(expectedCustomerId)).Returns(customer);

        // Act
        var contoller = new CustomerController(_logger, _customerRepository);
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
        var contoller = new CustomerController(_logger, _customerRepository);
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
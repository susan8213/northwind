using Microsoft.Extensions.Logging;
using Northwind.Access;
using Northwind.Exceptions;
using Northwind.Models;
using Northwind.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Northwind.Tests;

[TestClass]
public class TestOrderService
{
    private OrderService _orderService;
    private ILogger<OrderService> _logger;
    private IOrderRepository _orderRepository;
    private IProductRepository _productRepository;
    private ICustomerRepository _customerRepository;

    [TestInitialize]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<OrderService>>();
        _orderRepository = Substitute.For<IOrderRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _customerRepository = Substitute.For<ICustomerRepository>();

        _orderService = new OrderService(_logger, _orderRepository, _productRepository, _customerRepository);
    }

    [TestMethod]
    public async Task Test_GetOrder_ShouldReturnOrder()
    {
        // Arrange
        int expectedOrderId = 12345;
        Order order = new Order {OrderID=expectedOrderId}; 
        _orderRepository.GetAsync(Arg.Is(expectedOrderId)).Returns(order);

        // Act
        var result = await _orderService.GetOrderAsync(expectedOrderId);

        // Assert
        Assert.AreEqual(expectedOrderId, result.OrderID);
    }

    [TestMethod]
    public async Task Test_GetOrder_ShouldThrowNotFound()
    {
        // Arrange
        int expectedOrderId = 12345;
        _orderRepository.GetAsync(Arg.Is(expectedOrderId)).ReturnsNull();

        // Act
        try
        {
            var result = await _orderService.GetOrderAsync(expectedOrderId);
            // Assert
            Assert.Fail("An exception should have been thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(NotFoundException));
        }

    }

    [TestMethod]
    public async Task Test_Create_ShouldReturnOrderId()
    {
        // Arrange
        int productId = 12345;
        string customerId = "12345";
        int newOrderId = 1;
        Product product = new Product {ProductID = productId, UnitsInStock=1};
        OrderDetail orderDetailToCreate = new OrderDetail { Product =  new Product {ProductID=productId}, Quantity = 1};
        Order orderToCreate = new Order {
            CustomerID = customerId,
            OrderDate = DateTime.Today,
            OrderDetails = new List<OrderDetail>() {orderDetailToCreate}
        };
        _customerRepository.Get(Arg.Is(customerId)).Returns(new Customer());
        _productRepository.GetAsync(Arg.Is(productId)).Returns(product);
        _orderRepository.CreateAsync(Arg.Is(orderToCreate)).Returns(newOrderId);

        // Act
        var result = await _orderService.CreateAsync(orderToCreate);

        // Assert
        Assert.AreEqual(result, newOrderId);
    }

    [TestMethod]
    public async Task Test_Create_NonExistCustomerId_ShouldThrowNotFound()
    {
        // Arrange
        int productId = 12345;
        string wrongCustomerId = "12345";
        OrderDetail orderDetailToCreate = new OrderDetail { Product = new Product {ProductID = productId} };
        Order orderToCreate = new Order {
            CustomerID = wrongCustomerId,
            OrderDate = DateTime.Today,
            OrderDetails = new List<OrderDetail>() {orderDetailToCreate}
        };
        _customerRepository.Get(Arg.Is(wrongCustomerId)).ReturnsNull();
        _productRepository.GetAsync(Arg.Is(productId)).ReturnsNull();
        _orderRepository.CreateAsync(Arg.Is(orderToCreate)).Returns(1);

        // Act
        try
        {
            await _orderService.CreateAsync(orderToCreate);
            // Assert
            Assert.Fail("An exception should have been thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(NotFoundException));
        }
    }

    [TestMethod]
    public async Task Test_Create_NonexistingProduct_ShouldThrowNotFound()
    {
        // Arrange
        int wrongProductId = 12345;
        string customerId = "12345";
        OrderDetail orderDetailToCreate = new OrderDetail { Product = new Product {ProductID = wrongProductId} };
        Order orderToCreate = new Order {
            CustomerID = customerId,
            OrderDate = DateTime.Today,
            OrderDetails = new List<OrderDetail>() {orderDetailToCreate}
        };
        _customerRepository.Get(Arg.Is(customerId)).Returns(new Customer());
        _productRepository.GetAsync(Arg.Is(wrongProductId)).ReturnsNull();
        _orderRepository.CreateAsync(Arg.Is(orderToCreate)).Returns(1);

        // Act
        try
        {
            await _orderService.CreateAsync(orderToCreate);
            // Assert
            Assert.Fail("An exception should have been thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(NotFoundException));
        }
    }

    [TestMethod]
    public async Task Test_Create_ProductOutOfStock_ShouldThrowValidationError()
    {
        // Arrange
        int productId = 12345;
        string customerId = "12345";
        Product product = new Product {ProductID = productId, UnitsInStock=0};
        OrderDetail orderDetailToCreate = new OrderDetail { Product =  new Product {ProductID=productId}, Quantity = 1};
        Order orderToCreate = new Order {
            CustomerID = customerId,
            OrderDate = DateTime.Today,
            OrderDetails = new List<OrderDetail>() {orderDetailToCreate}
        };
        _customerRepository.Get(Arg.Is(customerId)).Returns(new Customer());
        _productRepository.GetAsync(Arg.Is(productId)).Returns(product);
        _orderRepository.CreateAsync(Arg.Is(orderToCreate)).Returns(1);

        // Act
        try
        {
            await _orderService.CreateAsync(orderToCreate);
            // Assert
            Assert.Fail("An exception should have been thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(ValidationException));
        }
    }

    [TestMethod]
    public async Task Test_Update_AlreadyShipped_ShouldThrowValidationError()
    {
        // Arrange
        int originalOrderId = 12345;
        DateTime originalOrderDate = DateTime.Today;
        DateTime shippedTime = DateTime.UtcNow;
        Order originalOrder = new Order {OrderID=originalOrderId, OrderDate=originalOrderDate, ShippedDate=shippedTime};
        Order updatedOrder = new Order {OrderID=originalOrderId, ShippedDate=shippedTime};
        _orderRepository.UpdateAsync(Arg.Is(updatedOrder)).ReturnsNull();
        _orderRepository.GetAsync(Arg.Is(originalOrderId)).Returns(originalOrder);

        // Act
        try
        {
            await _orderService.UpdateAsync(updatedOrder);
            // Assert
            Assert.Fail("An exception should have been thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(ValidationException));
        }
        
    }

}
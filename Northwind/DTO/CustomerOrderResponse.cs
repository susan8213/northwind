using Northwind.Models;

namespace Northwind.DTO;

public class CustomerOrderResponse
{

    public IEnumerable<OrderResponse> Order {get; set;}
    public CustomerOrderResponse(IEnumerable<Order> orders)
    {
        List<OrderResponse> output = new List<OrderResponse>();
        foreach (var order in orders)
        {
            output.Add(new OrderResponse(order));
        }
        Order = output;
        
    }
}

public class OrderResponse
{
    public int OrderID { get; set; }
    public DateTime OrderDate {get; set; }
    public DateTime RequiredDate { get; set; }
    public DateTime ShippedDate { get; set; }
    public IEnumerable<OrderDetailResponse> OrderDetails {get; set; }

    public OrderResponse(Order order)
    {
        OrderID = order.OrderID;
        OrderDate = order.OrderDate;
        RequiredDate = order.RequiredDate;
        ShippedDate = order.ShippedDate;

        List<OrderDetailResponse> output = new List<OrderDetailResponse>();
        foreach (var orderDetail in order.OrderDetails)
        {
            output.Add(new OrderDetailResponse(orderDetail));
        }
        OrderDetails = output;
    }
}

public class OrderDetailResponse
{
    public string ProductName {get; set;}
    public double UnitPrice { get; set;}
    public int Quantity { get; set; }
    public double Discount { get; set; }
    public double TotalPrice { get; set; }

    public OrderDetailResponse(OrderDetail orderDetail)
    {
        ProductName = orderDetail.Product.ProductName;
        UnitPrice = orderDetail.UnitPrice;
        Quantity = orderDetail.Quantity;
        Discount = orderDetail.Discount;
        TotalPrice = orderDetail.TotalPrice;
    }
}
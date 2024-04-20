namespace Northwind.Models;

public class OrderDetail {

    public Product Product {get; set;}
    public double UnitPrice { get; set;}
    public int Quantity { get; set; }
    public double Discount { get; set; }
    public double TotalPrice 
    { 
        get => UnitPrice * Quantity * (1-Discount);
    }
}
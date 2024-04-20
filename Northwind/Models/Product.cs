namespace Northwind.Models;

public class Product {

    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public string QuantityPerUnit { get; set; }
    public double UnitPrice { get; set; }
    public int UnitInStock { get; set; }
    public Category? Category {get; set; }
    public Supplier? Supplier { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Northwind.Attributes;

namespace Northwind.DTO;

public class CreateOrderRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(5)]
    public string CustomerID {get; set;}

    [Required]
    [FromNow]
    public DateTime? RequiredDate { get; set; }
    
    [FromNow]
    public DateTime? ShippedDate { get; set; }


    [Required]
    [MinLength(1)]
    public ICollection<CreateOrderDetailRequest> OrderDetails { get; set; }
}

public class CreateOrderDetailRequest
{
    [Required]
    public int ProductID {get; set;}

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, 1)]
    public double Discount { get; set; }
}
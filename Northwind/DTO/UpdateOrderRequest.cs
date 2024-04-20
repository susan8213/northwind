using System.ComponentModel.DataAnnotations;
using Northwind.Attributes;

namespace Northwind.DTO;

public class UpdateOrderRequest
{

    [FromNow]
    public DateTime? RequiredDate { get; set; }
    
    [Required]
    [FromNow]
    public DateTime? ShippedDate { get; set; }

}
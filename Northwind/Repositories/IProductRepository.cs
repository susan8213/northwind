using Northwind.Models;

namespace Northwind.Access;

public interface IProductRepository
{
    Task<Product> GetAsync(int id);
}
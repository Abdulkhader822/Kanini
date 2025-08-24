// Interface/IProduct.cs
using ProSportsStore.Models;

namespace ProSportsStore.Interface
{
    public interface IProduct
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(int id);
        Task<Product> AddProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);

        Task<IEnumerable<Product>> SearchProducts(string keyword);
        Task<IEnumerable<Product>> FilterByPrice(decimal minPrice, decimal maxPrice);

    }
}

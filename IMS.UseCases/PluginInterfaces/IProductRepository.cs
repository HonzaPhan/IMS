using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
        Task EditProductAsync(Product product);
        Task DeleteProductByIdAsync(int id);
    }
}

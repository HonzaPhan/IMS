using IMS.CoreBusiness;
using IMS.UseCases.Inventories;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Products
{
    public class ViewProductsByNameUseCase : IViewProductsByNameUseCase
    {
        private readonly IProductRepository _productRepository;
        public ViewProductsByNameUseCase(IProductRepository ProductRepository)
        {
            _productRepository = ProductRepository;
        }

        public async Task<IEnumerable<Product>> ExecuteAsync(string name = "")
        {
            return await _productRepository.GetProductsByNameAsync(name);
        }
    }
}

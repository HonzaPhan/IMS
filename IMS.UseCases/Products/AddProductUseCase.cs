using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Products
{
    public class AddProductUseCase : IAddProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public AddProductUseCase(IProductRepository ProductRepository)
        {
            _productRepository = ProductRepository;
        }

        public async Task ExecuteAsync(Product product)
        {
            await _productRepository.CreateProductAsync(product);
        }
    }
}

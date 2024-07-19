using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products.Interfaces;

namespace IMS.UseCases.Products
{
    public class ViewProductByIdUseCase : IViewProductByIdUseCase
    {
        private readonly IProductRepository _productRepository;
        public ViewProductByIdUseCase(IProductRepository ProductRepository)
        {
            _productRepository = ProductRepository;
        }

        public async Task<Product?> ExecuteAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }
    }
}

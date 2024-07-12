using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> _products;
        
        public ProductRepository()
        {
            _products = new List<Product>()
            {
                    new Product { ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 2000},
                    new Product { ProductId = 2, ProductName = "Car", Quantity = 10, Price = 22000},
            };
        }

        public Task AddProductAsync(Product Product)
        {
            if (_products.Any(i => i.ProductName.Equals(Product.ProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }
            
            int maxId = _products.Max(i => i.ProductId);
            Product.ProductId = maxId + 1;

            _products.Add(Product);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await Task.FromResult(_products);
            }

            return _products.Where(i => i.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }


        public Task EditProductAsync(Product Product)
        {
            if(_products.Any(i => i.ProductId != Product.ProductId && i.ProductName.Equals(Product.ProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            Product? existingProduct = _products.FirstOrDefault(i => i.ProductId == Product.ProductId);
            if (existingProduct == null)
            {
                return Task.CompletedTask;
            }

            existingProduct.ProductName = Product.ProductName;
            existingProduct.Quantity = Product.Quantity;
            existingProduct.Price = Product.Price;

            return Task.CompletedTask;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await Task.FromResult(_products.First(i => i.ProductId == id));
        }

        public Task DeleteProductByIdAsync(int id)
        {
            Product Product = _products.FirstOrDefault(i => i.ProductId == id);

            if (Product != null)
            {
                _products.Remove(Product);
            }

            return Task.CompletedTask;
        }
    }
}

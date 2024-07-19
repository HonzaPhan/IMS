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

        public Task CreateProductAsync(Product product)
        {
            if (_products.Any(i => i.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            int maxId = _products.Max(i => i.ProductId);
            product.ProductId = maxId + 1;

            _products.Add(product);

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


        public Task UpdateProductAsync(Product product)
        {
            if (_products.Any(i => i.ProductId != product.ProductId && i.ProductName.ToLower() == product.ProductName.ToLower())) return Task.CompletedTask;

            Product? existingProduct = _products.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Price = product.Price;
                existingProduct.ProductInventories = product.ProductInventories;
            }

            return Task.CompletedTask;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            Product? product = _products.FirstOrDefault(p => p.ProductId == id);
            Product newProduct = new Product();

            if (product != null)
            {
                newProduct.ProductId = product.ProductId;
                newProduct.ProductName = product.ProductName;
                newProduct.Quantity = product.Quantity;
                newProduct.Price = product.Price;
                newProduct.ProductInventories = new List<ProductInventory>();

                if (product.ProductInventories != null && product.ProductInventories.Count > 0)
                {
                    foreach (ProductInventory productInventory in product.ProductInventories)
                    {
                        ProductInventory newProductInventory = new ProductInventory
                        {
                            InventoryId = productInventory.InventoryId,
                            ProductId = productInventory.ProductId,
                            Product = product,
                            Inventory = new Inventory(),
                            InventoryQuantity = productInventory.InventoryQuantity,
                        };

                        if (productInventory.Inventory != null)
                        {
                            newProductInventory.Inventory.InventoryId = productInventory.Inventory.InventoryId;
                            newProductInventory.Inventory.InventoryName = productInventory.Inventory.InventoryName;
                            newProductInventory.Inventory.Quantity = productInventory.Inventory.Quantity;
                            newProductInventory.Inventory.Price = productInventory.Inventory.Price;
                        }

                        newProduct.ProductInventories.Add(newProductInventory);
                    }
                }
            }

            return await Task.FromResult(newProduct);
        }

        public Task DeleteProductByIdAsync(int id)
        {
            Product? Product = _products.FirstOrDefault(i => i.ProductId == id);

            if (Product != null)
            {
                _products.Remove(Product);
            }

            return Task.CompletedTask;
        }
    }
}

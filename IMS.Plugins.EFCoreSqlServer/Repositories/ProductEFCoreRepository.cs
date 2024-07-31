using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer.Repositories
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProductEFCoreRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateProductAsync(Product product)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            db.Products?.AddAsync(product);
            FlagInventoryUnchanged(product, db);
            await db.SaveChangesAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            Product? product = await db.Products.FindAsync(id);
            if (product != null) return product;

            return new Product();
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            return await db.Products.Where(i => i.ProductName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            Product? existingProduct = await db.Products.FindAsync(product.ProductId);
            if (existingProduct == null) return;

            existingProduct.ProductName = product.ProductName;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Price = product.Price;
            existingProduct.ProductInventories = product.ProductInventories;

            FlagInventoryUnchanged(product, db);

            await db.SaveChangesAsync();
        }

        public async Task DeleteProductByIdAsync(int id)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            Product? product = db.Products?.Find(id);
            if (product == null) return;

            db.Products?.Remove(product);
            await db.SaveChangesAsync();
        }

        private void FlagInventoryUnchanged(Product product, AppDbContext db)
        {
            if (product?.ProductInventories != null && product.ProductInventories.Count > 0)
            {
                foreach (ProductInventory productInventory in product.ProductInventories)
                {
                    if (productInventory.Inventory != null)
                    {
                        db.Entry(productInventory.Inventory).State = EntityState.Unchanged;
                    }
                }
            }

        }
    }
}

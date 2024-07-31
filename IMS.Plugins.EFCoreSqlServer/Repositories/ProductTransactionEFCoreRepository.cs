using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer.Repositories
{
    public class ProductTransactionEFCoreRepository : IProductTransactionRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public ProductTransactionEFCoreRepository(IDbContextFactory<AppDbContext> contextFactory, IProductRepository productRepository, IInventoryTransactionRepository inventoryTransactionRepository, IInventoryRepository inventoryRepository)
        {
            _contextFactory = contextFactory;
            _productRepository = productRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy, double price)
        {
            AppDbContext db = _contextFactory.CreateDbContext();
            Product? prod = await _productRepository.GetProductByIdAsync(product.ProductId);

            if (prod != null)
            {
                foreach (ProductInventory pi in prod.ProductInventories)
                {
                    if (pi.Inventory != null)
                    {
                        // add inventory transaction
                        await _inventoryTransactionRepository.ProduceAsync(productionNumber, pi.Inventory, pi.InventoryQuantity * quantity, doneBy, -1);

                        Inventory inv = await _inventoryRepository.GetInventoryByIdAsync(pi.Inventory.InventoryId);

                        // decrease the quantity of inventories
                        if (inv != null)
                        {
                            inv.Quantity -= pi.InventoryQuantity * quantity;
                            await _inventoryRepository.UpdateInventoryAsync(inv);
                        }
                    }
                }
            }

            // add product transaction
            db.ProductTransactions?.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
            });

            await db.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            AppDbContext db = _contextFactory.CreateDbContext();

            db.ProductTransactions?.Add(new ProductTransaction
            {
                SoNumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.SellProduct,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = unitPrice,
            });

            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType)
        {
            AppDbContext db = _contextFactory.CreateDbContext();

            IQueryable<ProductTransaction> query = from pt in db.ProductTransactions
                                                    join prod in db.Products on pt.ProductId equals prod.ProductId
                                                      where
                                                          (string.IsNullOrWhiteSpace(productName) || prod.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                                                          && (!dateFrom.HasValue || pt.TransactionDate >= dateFrom.Value.Date)
                                                          && (!dateTo.HasValue || pt.TransactionDate <= dateTo.Value.Date)
                                                          && (!productTransactionType.HasValue || pt.ActivityType == productTransactionType)
                                                      select new ProductTransaction
                                                      {
                                                          Product = prod,
                                                          ProductTransactionId = pt.ProductTransactionId,
                                                          SoNumber = pt.SoNumber,
                                                          ProductionNumber = pt.ProductionNumber,
                                                          ProductId = pt.ProductId,
                                                          QuantityBefore = pt.QuantityBefore,
                                                          ActivityType = pt.ActivityType,
                                                          QuantityAfter = pt.QuantityAfter,
                                                          TransactionDate = pt.TransactionDate,
                                                          DoneBy = pt.DoneBy,
                                                          UnitPrice = pt.UnitPrice,
                                                      };

            return await query.Include(pt => pt.Product).ToListAsync();
        }
    }
}

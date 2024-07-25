using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new List<ProductTransaction>();

        private readonly IProductRepository _productRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public ProductTransactionRepository(IProductRepository productRepository, IInventoryTransactionRepository inventoryTransactionRepository, IInventoryRepository inventoryRepository)
        {
            _productRepository = productRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy, double price)
        {
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
            _productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
            });
        }

        public Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            _productTransactions.Add(new ProductTransaction
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

            return Task.CompletedTask;
        }
    }
}

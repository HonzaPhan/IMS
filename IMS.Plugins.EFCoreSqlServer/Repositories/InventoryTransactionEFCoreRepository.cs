using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer.Repositories
{
    public class InventoryTransactionEFCoreRepository : IInventoryTransactionRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public InventoryTransactionEFCoreRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            AppDbContext db = _contextFactory.CreateDbContext();

            IQueryable<InventoryTransaction> query = from it in db.InventoryTransactions
                                                      join inv in db.Inventories on it.InventoryId equals inv.InventoryId
                                                      where
                                                          (string.IsNullOrWhiteSpace(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0)
                                                          && (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date)
                                                          && (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date)
                                                          && (!inventoryTransactionType.HasValue || it.ActivityType == inventoryTransactionType)
                                                      select it;

            return await query.Include(it => it.Inventory).ToListAsync();
        }

        public async Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            AppDbContext db = _contextFactory.CreateDbContext();

            db.InventoryTransactions?.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price,
            });

            await db.SaveChangesAsync();
        }

        public async Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            AppDbContext db = _contextFactory.CreateDbContext();

            db.InventoryTransactions?.Add(new InventoryTransaction
            {
                PoNumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                QuantityAfter = inventory.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price,
            });

            await db.SaveChangesAsync();
        }
    }
}

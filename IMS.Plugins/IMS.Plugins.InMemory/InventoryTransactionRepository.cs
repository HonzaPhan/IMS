using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly IInventoryRepository _inventoryRepository;
        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            IEnumerable<Inventory> inventories = (await _inventoryRepository.GetInvetoriesByNameAsync(string.Empty)).ToList();

            IEnumerable<InventoryTransaction> query = from it in _inventoryTransactions 
                        join inv in inventories on it.InventoryId equals inv.InventoryId 
                        where 
                            (string.IsNullOrWhiteSpace(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0)
                            && (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date)
                            && (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date)
                            && (!inventoryTransactionType.HasValue || it.ActivityType == inventoryTransactionType)
                        select new InventoryTransaction
                        {
                            Inventory = inv,
                            InventoryTransactionId = it.InventoryTransactionId,
                            PoNumber = it.PoNumber,
                            InventoryId = it.InventoryId,
                            QuantityBefore = it.QuantityBefore,
                            ActivityType = it.ActivityType,
                            QuantityAfter = it.QuantityAfter,
                            TransactionDate = it.TransactionDate,
                            DoneBy = it.DoneBy,
                            UnitPrice = it.UnitPrice,
                        };

            return query;
        }

        public Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            _inventoryTransactions.Add(new InventoryTransaction
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

            return Task.CompletedTask;
        }

        public Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            _inventoryTransactions.Add(new InventoryTransaction
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

            return Task.CompletedTask;
        }
    }
}

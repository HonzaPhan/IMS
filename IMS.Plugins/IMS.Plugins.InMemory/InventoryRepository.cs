using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryRepository : IInventoryRepository
    {
        private List<Inventory> _inventories;
        
        public InventoryRepository()
        {
            _inventories = new List<Inventory>()
            {
                    new Inventory { InventoryId = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2},
                    new Inventory { InventoryId = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15},
                    new Inventory { InventoryId = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8},
                    new Inventory { InventoryId = 4, InventoryName = "Bike Pedels", Quantity = 20, Price = 1},
            };
        }

        public Task AddInventoryAsync(Inventory inventory)
        {
            if (_inventories.Any(i => i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }
            
            int maxId = _inventories.Max(i => i.InventoryId);
            inventory.InventoryId = maxId + 1;

            _inventories.Add(inventory);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Inventory>> GetInvetoriesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await Task.FromResult(_inventories);
            }

            return _inventories.Where(i => i.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer.Repositories
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public InventoryEFCoreRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateInventoryAsync(Inventory inventory)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            db.Inventories?.AddAsync(inventory);
            await db.SaveChangesAsync();
        }

        public async Task<Inventory> GetInventoryByIdAsync(int id)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            Inventory? inventory = await db.Inventories.FindAsync(id);
            if (inventory != null) return inventory;

            return new Inventory();
        }

        public async Task<IEnumerable<Inventory>> GetInvetoriesByNameAsync(string name)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            return await db.Inventories.Where(i => i.InventoryName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            Inventory? existingInventory = await db.Inventories.FindAsync(inventory.InventoryId);
            if (existingInventory == null) return;

            existingInventory.InventoryName = inventory.InventoryName;
            existingInventory.Quantity = inventory.Quantity;
            existingInventory.Price = inventory.Price;

            await db.SaveChangesAsync();
        }

        public async Task DeleteInventoryByIdAsync(int id)
        {
            using AppDbContext db = _contextFactory.CreateDbContext();

            Inventory? inventory = db.Inventories?.Find(id);
            if (inventory == null) return;

            db.Inventories?.Remove(inventory);
            await db.SaveChangesAsync();
        }
    }
}

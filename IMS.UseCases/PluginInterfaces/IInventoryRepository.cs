using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryRepository
    {
        Task CreateInventoryAsync(Inventory inventory);
        Task<Inventory> GetInventoryByIdAsync(int id);
        Task<IEnumerable<Inventory>> GetInvetoriesByNameAsync(string name);
        Task UpdateInventoryAsync(Inventory inventory);
        Task DeleteInventoryByIdAsync(int id);
    }
}

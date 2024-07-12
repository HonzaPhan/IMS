using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryRepository
    {
        Task AddInventoryAsync(Inventory inventory);
        Task<Inventory> GetInventoryByIdAsync(int id);
        Task<IEnumerable<Inventory>> GetInvetoriesByName(string name);
        Task EditInventoryAsync(Inventory inventory);
        Task DeleteInventoryByIdAsync(int id);
    }
}

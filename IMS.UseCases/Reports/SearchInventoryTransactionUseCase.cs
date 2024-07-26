using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Reports.Interfaces;

namespace IMS.UseCases.Reports
{
    public class SearchInventoryTransactionUseCase : ISearchInventoryTransactionUseCase
    {
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public SearchInventoryTransactionUseCase(IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> ExecuteAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            if (dateTo.HasValue) dateTo = dateTo.Value.AddDays(1);

            return await _inventoryTransactionRepository.GetInventoryTransactionsAsync(inventoryName, dateFrom, dateTo, inventoryTransactionType);
        }
    }
}

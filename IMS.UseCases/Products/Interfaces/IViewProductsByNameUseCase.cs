using IMS.CoreBusiness;

namespace IMS.UseCases.Inventories
{
    public interface IViewProductsByNameUseCase
    {
        Task<IEnumerable<Product>> ExecuteAsync(string name = "");
    }
}
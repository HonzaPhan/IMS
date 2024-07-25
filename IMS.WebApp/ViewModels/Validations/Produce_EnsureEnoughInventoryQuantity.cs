using IMS.CoreBusiness;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels.Validations
{
    public class Produce_EnsureEnoughInventoryQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ProduceViewModel? produceViewModel = validationContext.ObjectInstance as ProduceViewModel;

            if (produceViewModel != null && produceViewModel.Product != null && produceViewModel.Product.ProductInventories != null)
            {
                foreach (ProductInventory pi in produceViewModel.Product.ProductInventories)
                {
                    if (pi.Inventory != null && pi.InventoryQuantity * produceViewModel.QuantityToProduce > pi.Inventory.Quantity)
                    {
                        return new ValidationResult($"The inventory ({pi.Inventory.InventoryName}) is not enough to produce {produceViewModel.QuantityToProduce} products",
                            memberNames: [validationContext.MemberName]);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}

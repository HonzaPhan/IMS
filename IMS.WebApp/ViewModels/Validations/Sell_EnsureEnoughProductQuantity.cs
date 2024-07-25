using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels.Validations
{
    public class Sell_EnsureEnoughProductQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            SellViewModel? sellViewModel = validationContext.ObjectInstance as SellViewModel;

            if (sellViewModel != null && sellViewModel.Product != null && sellViewModel.Product.Quantity < sellViewModel.QuantityToSell)
            {
                return new ValidationResult($"There isn't enough product. There is only {sellViewModel.Product.Quantity} in the warehouse.",
                               memberNames: [validationContext.MemberName]);
            }

            return ValidationResult.Success;
        }
    }
}

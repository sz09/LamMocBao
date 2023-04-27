using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LamMocBaoWeb.Utilities
{
    public static class ModelValidation
    {
        public static bool IsValid(this ModelStateDictionary modelState)
        {
            if (modelState.Count > 0)
            {
                var errors = modelState.Where(d => d.Value.ValidationState == ModelValidationState.Invalid);
                foreach (var item in errors)
                {
                    var errorMessages = item.Value.Errors.Select(d => d.ErrorMessage);
                    if (errorMessages.Any())
                    {
                        modelState.AddModelError("ValidateErrors:" + item.Key, string.Join(", ", errorMessages));
                    }
                }
                return false;
            }

            return true;
        }
    }
}

using Mcsg.Identity.Api.DTOs.Request;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Attributes
{
    public class RequireEmailOrPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = (BaseUserReq)validationContext.ObjectInstance;

            return string.IsNullOrWhiteSpace(user.Email) && string.IsNullOrWhiteSpace(user.Phone)
                ? new ValidationResult("Email or Phone is required.")
                : ValidationResult.Success;
        }
    }
}

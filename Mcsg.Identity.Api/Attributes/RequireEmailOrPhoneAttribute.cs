using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Attributes;

using Requests;

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

using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Common.DataAnnotationExtensions;

using Models;

public class RequireEmailOrPhoneAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var emailPhoneRequest = (BaseEmailPhoneRequest)validationContext.ObjectInstance;

        return string.IsNullOrWhiteSpace(emailPhoneRequest.Email) && string.IsNullOrWhiteSpace(emailPhoneRequest.PhoneNumber)
            ? new ValidationResult("Email or Phone is required.")
            : ValidationResult.Success;
    }
}

using System.Text.RegularExpressions;

namespace Mcsg.Api.Areas.Identity.Helpers;

public class FieldValidation
{
    public static bool EmailValidate(string email)
    {
        Regex validateEmailRegex = new Regex("^\\S+@\\S+\\.\\S+$");

        return validateEmailRegex.IsMatch(email);
    }
}

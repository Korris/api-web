using System.Text.RegularExpressions;

namespace Mcsg.Identity.Api.Helpers;

public class FieldValidation
{
    public static bool EmailValidate(string email)
    {
        Regex validateEmailRegex = new Regex("^\\S+@\\S+\\.\\S+$");

        return validateEmailRegex.IsMatch(email);
    }
}

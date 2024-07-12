using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Mcsg.Lib.Common.Mail;

public static class EmailHelper
{
    public static bool IsValid(string email)
    {
        // Regular expression for validating an Email
        string emailPattern = @"^[a-zA-Z0-9_+&*-]+(?:\.[a-zA-Z0-9_+&*-]+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,7}$";
        Regex regex = new Regex(emailPattern);
        return regex.IsMatch(email);
    }
    public static string[] ParseEmail(string email)
    {
        try
        {
            MailAddress addr = new MailAddress(email);
            string username = addr.User;
            string domain = addr.Host;
            return new string[] { username, domain };
        }
        catch (Exception ex)
        {
            return null; // Invalid email address format
        }
    }
}

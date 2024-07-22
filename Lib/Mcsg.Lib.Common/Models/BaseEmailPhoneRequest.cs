using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Common.Models;

using DataAnnotationExtensions;

public abstract class BaseEmailPhoneRequest
{
    [EmailAddress]
    [RequireEmailOrPhone]
    public string Email { get; set; }
    [RequireEmailOrPhone]
    public string PhoneNumber { get; set; }
}

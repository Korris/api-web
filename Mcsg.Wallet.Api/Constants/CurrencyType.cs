using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Api.Constants;

public enum CurrencyType
{
    [Display(Name = "VND")]
    VND = 0,
    [Display(Name = "USD")]
    USD = 1
}

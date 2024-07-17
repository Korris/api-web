using System.ComponentModel.DataAnnotations;

namespace Mcsg.Comic.Api.Models.Earning;

public class EarningAffiliateCodeR
{
    [Required]
    public string HashId { get; set; }
    [Required]
    public string Type { get; set; }
}

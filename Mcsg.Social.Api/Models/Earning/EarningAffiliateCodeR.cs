using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Models.Earning
{
    public class EarningAffiliateCodeR
    {
        [Required]
        public string HashId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}

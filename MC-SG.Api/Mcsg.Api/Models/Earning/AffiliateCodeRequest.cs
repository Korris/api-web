using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Models.Earning
{
    public class AffiliateCodeRequest
    {
        [Required]
        public string HashId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}

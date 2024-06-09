using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.DTOs
{
    public class SearchSoundReq : BackgroundMediaLoadReq
    {
        [Required]
        public string Keyword { get; set; }
    }
}

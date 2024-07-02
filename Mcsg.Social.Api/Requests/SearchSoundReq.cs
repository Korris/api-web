using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests
{
    public class SearchSoundReq : BackgroundMediaLoadReq
    {
        [Required]
        public string? Keyword { get; set; }
    }
}

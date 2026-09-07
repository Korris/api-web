using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Document.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Story.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}

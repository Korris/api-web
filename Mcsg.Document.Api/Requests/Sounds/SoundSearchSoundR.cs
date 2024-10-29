using System.ComponentModel.DataAnnotations;

namespace Mcsg.Document.Api.Requests;

public class SoundSearchSoundR : SoundBackgroundMediaLoadR
{
    [Required]
    public string? Keyword { get; set; }
}

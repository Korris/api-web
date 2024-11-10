using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class BackgroundMedia : AuditableEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? Url { get; set; }

    public string? Thumbnail { get; set; }

    public string? ArtistName { get; set; }

    public int DurationSeconds { get; set; }

    public int Order { get; set; }

    [InverseProperty("BackgroundMedia")]
    public virtual ICollection<BackgroundMediaPost> BackgroundMediaPosts { get; set; } = new List<BackgroundMediaPost>();
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

[Table("BackgroundMedias")]
public partial class BackgroundMedia : AuditableEntity
{
    public string? Title { get; set; }
    public string? Url { get; set; }
    public string? Thumbnail { get; set; }
    public string? ArtistName { get; set; }
    public int DurationSeconds { get; set; }
    public int Order { get; set; }
}

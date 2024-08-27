using System.ComponentModel.DataAnnotations;

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
}

using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class StoryRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}

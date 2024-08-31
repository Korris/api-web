using System.ComponentModel.DataAnnotations;

namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class StoryRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}

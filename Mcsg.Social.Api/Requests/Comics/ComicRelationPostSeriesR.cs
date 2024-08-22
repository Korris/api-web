using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;

public class ComicRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}

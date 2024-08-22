using System.ComponentModel.DataAnnotations;

namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class ComicRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class ComicRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}

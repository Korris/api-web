using System.ComponentModel.DataAnnotations;

namespace Mcsg.Comic.Api.Requests;

public class ComicRelationPostSeriesR : BasePageResultR
{
    [Required]
    public string? HashId { get; set; }
}

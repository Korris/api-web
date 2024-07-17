using System.ComponentModel.DataAnnotations;

namespace Mcsg.Story.Api.Requests;

public class ComicRelationPostSeriesR : BasePageResultR
{
    [Required]
    public string? HashId { get; set; }
}

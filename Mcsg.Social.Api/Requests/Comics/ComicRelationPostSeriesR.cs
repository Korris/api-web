using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests;

public class ComicRelationPostSeriesR : BasePageResultR
{
    [Required]
    public string? HashId { get; set; }
}

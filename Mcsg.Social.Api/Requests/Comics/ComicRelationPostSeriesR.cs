using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests;

public class ComicRelationPostSeriesR : BasePageResultReq
{
    [Required]
    public string? HashId { get; set; }
}

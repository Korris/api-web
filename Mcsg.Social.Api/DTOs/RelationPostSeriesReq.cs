using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.DTOs
{
    public class RelationPostSeriesReq : BasePageResultReq
    {
        [Required]
        public string HashId { get; set; }
    }
}

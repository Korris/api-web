using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests
{
    public class RelationPostSeriesReq : BasePageResultReq
    {
        [Required]
        public string HashId { get; set; }
    }
}

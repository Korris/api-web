using System.ComponentModel;

namespace Mcsg.Api.DTOs
{
    public class ReactionByTargetRequest
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        public string Type { get; set; }
    }
}

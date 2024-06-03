using Mcsg.Identity.Api.Attributes;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class BaseUserReq
    {
        [RequireEmailOrPhone]
        public string Email { get; set; }

        [RequireEmailOrPhone]
        public string Phone { get; set; }
    }
}

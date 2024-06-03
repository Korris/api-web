using Mcsg.Lib.Model.Enums;

namespace Mcsg.Admin.Api.DTOs.Users
{
    public class UserListRequest : GetByPageReq
    {
        public string SearchName { get; set; }
        public UserStatus? Status { get; set; }
    }
}

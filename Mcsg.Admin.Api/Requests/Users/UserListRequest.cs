namespace Mcsg.Admin.Api.Requests
{
    using Common.Core.Enums;

    public class UserListRequest : GetByPageReq
    {
        public string SearchName { get; set; }
        public UserStatus? Status { get; set; }
    }
}

using Mcsg.Admin.Api.DTOs.Users;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Admin.Api.Services.Interface
{
    public interface IUserService
    {
        Task<PagedResults<UserRespone>> GetListUserAsync(UserListRequest request);
        Task<bool> ChangeStatusAsync(Guid id, UserStatusReq request);
        Task<Guid> CreateAdminAsync(CreateAdminReq request);
        Task<bool> DeleteUserAsync(Guid id);
    }
}

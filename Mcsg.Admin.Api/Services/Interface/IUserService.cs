namespace Mcsg.Admin.Api.Services.Interface
{
    using Dtos;
    using Lib.Data.Entities.Common;
    using Requests;

    public interface IUserService
    {
        Task<PagedResults<UserRespone>> GetListUserAsync(UserListRequest request);
        Task<bool> ChangeStatusAsync(Guid id, UserStatusReq request);
        Task<Guid> CreateAdminAsync(CreateAdminReq request);
        Task<bool> DeleteUserAsync(Guid id);
    }
}

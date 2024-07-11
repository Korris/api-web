namespace Mcsg.Admin.Api.Services.Interface
{
    using Common.SeedWork.Responses;
    using Dtos;
    using Requests;

    public interface IUserService
    {
        Task<PagedResponse<UserRespone>> GetListUserAsync(UserListRequest request);
        Task<bool> ChangeStatusAsync(Guid id, UserStatusReq request);
        Task<Guid> CreateAdminAsync(CreateAdminReq request);
        Task<bool> DeleteUserAsync(Guid id);
    }
}

namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IUserService
{
    Task<PagedResponse<UserSearchResponse>> SearchUserbyKeyword(SmartLookupSearchUserR input);
}

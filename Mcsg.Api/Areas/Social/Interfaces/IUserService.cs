namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public interface IUserService
{
    Task<PagedResponse<UserSearchResponse>> SearchUserbyKeyword(SmartLookupSearchUserR input);
}

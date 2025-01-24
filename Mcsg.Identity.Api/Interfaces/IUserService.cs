namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Requests;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Requests;
using Response;
using Services;

public interface IUserService
{
    string GenerateReferralCode();
    Task<PagedResponse<UserFollowedResponse>> GetFollowingProfilesAsync(UserNamePagingR req);
    Task<PagedResponse<UserFollowedResponse>> GetFollowedProfileAsync(UserNamePagingR req);
    Task<User.FullProfileDto> GetUserAsync(BaseR request);
    Task<User.FullProfileDto> GetUserAsync(string userName, Guid? userFollowerId);
    Task<UserProfileAvatarResponse?> GetUserAvatar(Guid userId);
    Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateR request);
    Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateR request);
    Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string? name);
    Task<List<SimilarProfile>> GetSimilarNameAsync(string name);
    Task<List<UserFollowedResponse>> GetSuggestedProfilesNotFollowedAsync(Guid? userId, string userName);
    Task<User.FullProfileDto> UpdateUserProfile(UserProfileUpdateR req);
}

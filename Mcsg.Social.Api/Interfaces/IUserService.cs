namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IUserService
{
    Task<UserProfileResponse> GetCurrentUserAsync();
    Task<UserProfileResponse> GetUserByUserNameAsync(string profileName);
    Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdateR req);
    Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateR userAvatarUpdateRequest);
    Task<UserProfileAvatarResponse> GetUserAvatar(Guid userId);
    Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateR userCoverPhotoUpdateRequest);
    Task<List<SimilarProfile>> GetSimilarNameAsync(string name);
    Task SyncWalletUserReward(Guid userId, float point, RewardType type);
    Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string? name);
    Task<PagedResponse<UserSearchResponse>> SearchUserbyKeyword(SmartLookupSearchUserR input);
    Task<List<UserFollowedResponse>> GetSuggestedProfilesNotFollowedAsync();
    Task<PagedResponse<UserFollowedResponse>> GetFollowingProfilesAsync(BasePageResultR request);
    Task<bool> FollowUserAsync(Guid userId);
    Task<bool> UnFollowUserAsync(Guid userId);
    Task<PagedResponse<UserFollowedResponse>> GetFollowedProfileAsync(BasePageResultR req);
}

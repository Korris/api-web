namespace Mcsg.Identity.Api.Interfaces;

using Common.SeedWork.Responses;
using Identity.Api.Requests;
using Identity.Api.Response;
using Identity.Api.Services;

public interface IUserService
{
    string GenerateReferralCode();
    Task<bool> ConfirmEmailAsync(string email);
    Task<bool> ConfirmPhoneNumberAsync(string phone);
    Task<UserProfileResponse> GetUserByUserNameAsync(string userName);
    Task<PagedResponse<UserFollowedResponse>> GetFollowingProfilesAsync(UserNamePagingR req);
    Task<PagedResponse<UserFollowedResponse>> GetFollowedProfileAsync(UserNamePagingR req);
    Task<UserProfileResponse> GetCurrentUserAsync();
    Task<bool> FollowUserAsync(Guid userId);
    Task<bool> UnFollowUserAsync(Guid userId);
    Task<UserProfileAvatarResponse?> GetUserAvatar(Guid userId);
    Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateR userAvatarUpdateRequest);
    Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string? name);
    Task<List<SimilarProfile>> GetSimilarNameAsync(string name);
    Task<List<UserFollowedResponse>> GetSuggestedProfilesNotFollowedAsync(string userName);
    Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateR userCoverPhotoUpdateRequest);
    Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdateR req);
}

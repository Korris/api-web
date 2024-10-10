namespace Mcsg.Identity.Api.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Requests;
using Response;
using Services;

public interface IUserService
{
    string GenerateReferralCode();
    Task<bool> ConfirmEmailAsync(string email);
    Task<bool> ConfirmPhoneNumberAsync(string phone);
    Task<User.FullProfileDto> GetUserByUserNameAsync(string userName);
    Task<PagedResponse<UserFollowedResponse>> GetFollowingProfilesAsync(UserNamePagingR req);
    Task<PagedResponse<UserFollowedResponse>> GetFollowedProfileAsync(UserNamePagingR req);
    Task<User.FullProfileDto> GetCurrentUserAsync();
    Task<UserProfileAvatarResponse?> GetUserAvatar(Guid userId);
    Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateR request);
    Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateR request);
    Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string? name);
    Task<List<SimilarProfile>> GetSimilarNameAsync(string name);
    Task<List<UserFollowedResponse>> GetSuggestedProfilesNotFollowedAsync(string userName);
    Task<User.FullProfileDto> UpdateUserProfile(UserProfileUpdateR req);
}

using Mcsg.Api.Models;
using Mcsg.Lib.Common.Enums;

namespace Mcsg.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse> GetCurrentUserAsync();
        Task<UserProfileResponse> GetUserByUserNameAsync(string profileName);
        Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdateRequest userProfileUpdateRequest);
        Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateRequest userAvatarUpdateRequest);
        Task<UserProfileAvatarResponse> GetUserAvatar(Guid userId);
        Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateRequest userCoverPhotoUpdateRequest);
        Task<List<SimilarProfile>> GetSimilarNameAsync(string name);
        Task SyncWalletUserReward(Guid userId, float point, RewardType type);
    }
}

using Mcsg.Lib.Common.Enums;

namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Lib.Data.Entities.Common;
    using Models;

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
        Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string name);
        Task<PagedResults<UserSearchResponse>> SearchUserbyKeyword(SearchUserReq input);
    }
}

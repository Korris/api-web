using Mcsg.Lib.Common.Enums;

namespace Mcsg.Social.Api.Interfaces
{
    using Lib.Data.Entities.Common;
    using Models;
    using Requests;

    public interface IUserService
    {
        Task<UserProfileResponse> GetCurrentUserAsync();
        Task<UserProfileResponse> GetUserByUserNameAsync(string profileName);
        Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdateR req);
        Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateRequest userAvatarUpdateRequest);
        Task<UserProfileAvatarResponse> GetUserAvatar(Guid userId);
        Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateRequest userCoverPhotoUpdateRequest);
        Task<List<SimilarProfile>> GetSimilarNameAsync(string name);
        Task SyncWalletUserReward(Guid userId, float point, RewardType type);
        Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string? name);
        Task<PagedResults<UserSearchResponse>> SearchUserbyKeyword(SearchUserReq input);
    }
}

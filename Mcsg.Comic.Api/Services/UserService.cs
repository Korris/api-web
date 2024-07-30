using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Constants;
using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Constants;
using Interfaces;
using Lib.Common.Models;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Models;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    private readonly DistributeManager _distributeManager;
    private readonly ILogger<UserService> _logger;

    public UserService(IRepository<User> userRepository,
        ICurrentUserService currentUserService,
        IConfiguration configuration,
        ILogger<UserService> logger,
        IRepository<SmartLookup> smartLookupRepository,
        DistributeManager distributeManager,
        IMcsgContext context,
        ISetting setting,
        IStorageClient sc)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _configuration = configuration;
        _logger = logger;
        _smartLookupRepository = smartLookupRepository;
        _distributeManager = distributeManager;
        _context = context;
        _setting = setting;
        _sc = sc;
    }

    public async Task<UserProfileResponse> GetCurrentUserAsync()
    {
        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == _currentUserService.Session.UserId);
        var userRespone = await CreateUserRespone(user);

        //Check first login
        if (user.LastLoginDate == null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(default);
        }

        return userRespone;
    }
    public async Task<UserProfileResponse> GetUserByUserNameAsync(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(p => p.UserName == userName);
        return await CreateUserResponeByUsername(user);
    }

    public async Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateR userAvatarUpdateRequest)
    {
        if (userAvatarUpdateRequest?.Avatar == null)
            throw new BadRequestException(ApiErrorCode.INVALID_AVATAR_IMAGE, ApiErrorMessage.INVALID_AVATAR_IMAGE);

        var fileExtension = Path.GetExtension(userAvatarUpdateRequest.Avatar.FileName);

        if (!Setting.FileExt.Images.Any(ext => ext == fileExtension.ToLower()))
            throw new BadRequestException(ApiErrorCode.INVALID_FILE_TYPE, ApiErrorMessage.INVALID_AVATAR_FILE_TYPE);

        string fileName = string.Empty;
        try
        {
            var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == _currentUserService.Session.UserId);

            var objectName = $"{Setting.MinioFolder.Image}/{userAvatarUpdateRequest.Avatar.FileName}";
            var isExistFile = await _sc.Strategy.StatObjectAsync(objectName, null);
            if (isExistFile != null)
            {
                fileName = GenerateNewFileName(userAvatarUpdateRequest.Avatar.FileName);
            }
            else
            {
                fileName = userAvatarUpdateRequest.Avatar.FileName;
            }

            user.Avatar = fileName;

            Stream newFormFile = null;
            using (var imageContent = userAvatarUpdateRequest.Avatar.OpenReadStream())
            {
                newFormFile = imageContent.ResizeImage(180, 180);
            }

            objectName = $"{Setting.MinioFolder.Image}/{fileName}";
            await _sc.Strategy.PutObject(newFormFile, objectName, null);
            newFormFile.Close();

            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(UpdateUserAvatar), userAvatarUpdateRequest);
            throw new BadRequestException(E500, ex.Message);
        }

        return new UserAvatarUpdateResponse() { Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(fileName) };
    }

    public async Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateR userCoverPhotoUpdateRequest)
    {
        if (userCoverPhotoUpdateRequest?.CoverPhoto == null)
            throw new BadRequestException(ApiErrorCode.INVALID_COVER_PHOTO_IMAGE, ApiErrorMessage.INVALID_COVER_PHOTO_IMAGE);

        var fileExtension = Path.GetExtension(userCoverPhotoUpdateRequest.CoverPhoto.FileName);
        if (!Setting.FileExt.Images.Any(ext => ext == fileExtension.ToLower()))
            throw new BadRequestException(ApiErrorCode.INVALID_FILE_TYPE, ApiErrorMessage.INVALID_COVER_PHOTO_FILE_TYPE);

        string fileName = string.Empty;
        try
        {
            var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == _currentUserService.Session.UserId);
            var objectName = $"{Setting.MinioFolder.Image}/{userCoverPhotoUpdateRequest.CoverPhoto.FileName}";
            var isExistFile = await _sc.Strategy.StatObjectAsync(objectName, null);
            if (isExistFile != null)
            {
                fileName = GenerateNewFileName(userCoverPhotoUpdateRequest.CoverPhoto.FileName);
            }
            else
            {
                fileName = userCoverPhotoUpdateRequest.CoverPhoto.FileName;
            }
            user.CoverPhoto = fileName;
            objectName = $"{Setting.MinioFolder.Image}/{fileName}";
            await _sc.Strategy.PutObject(userCoverPhotoUpdateRequest.CoverPhoto.OpenReadStream(), objectName, null);

            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(UpdateUserCoverPhoto), userCoverPhotoUpdateRequest);
            throw new BadRequestException(E500, ex.Message);
        }

        return new UserCoverPhotoUpdateResponse() { CoverPhoto = _setting.Minio.MediaApiUrl.ToPublicImageUrl(fileName) };
    }

    /// <summary>
    /// Only change email or phone number.
    /// Change both of them --> map to case change email and don't change the phone number.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public async Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdateR req)
    {
        var vr = new UserProfileUpdateV().Validate(req);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        var profileName = req.ProfileName?.Trim();
        if (string.IsNullOrWhiteSpace(profileName))
        {
            throw new BadRequestException(ApiErrorCode.PROFILE_NAME_NOT_EMPTY, ApiErrorMessage.PROFILE_NAME_NOT_EMPTY);
        }

        var user = await _context.Users.FindAsync(_currentUserService.Session.UserId);
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        if (user.ProfileName != profileName)
        {
            var smartLookup = await _context.SmartLookups.FirstOrDefaultAsync(p => p.Keyword == user.ProfileName && p.KeywordType == LookupKeywordType.People);
            if (smartLookup != null)
            {
                smartLookup.Keyword = profileName;
            }

            user.ProfileName = profileName;
        }

        var userName = req.UserName?.Trim();
        if (!string.IsNullOrWhiteSpace(userName))
        {
            var userNameHistory = await _context.UserNameHistories.FirstOrDefaultAsync(p => p.UserName == userName);
            if (userNameHistory == null)
            {
                if (!user.IsPremium && userName.Length <= Common.SeedWork.Constants.Validator.UserNamePremium.Max)
                {
                    throw new BadRequestException(ApiErrorCode.NEED_PREMIUM_TO_EDIT, ApiErrorMessage.NEED_PREMIUM_TO_EDIT);
                }

                userNameHistory = new UserNameHistory
                {
                    UserId = user.Id,
                    UserName = userName,
                    CreatedBy = user.Id
                };

                await _context.UserNameHistories.AddAsync(userNameHistory);
                var postToUpdate = await _context.ComicPosts.ToListAsync();

                Parallel.ForEach(postToUpdate, post =>
                {
                    if (post.Body != null && user.UserName != null)
                    {
                        post.Body = post.Body.ReplaceMentionUserNameInHtml(user.UserName, userName);
                    }
                });
            }
            else if (userNameHistory.UserId != user.Id)
            {
                throw new BadRequestException(ApiErrorCode.DUPLICATE_USERNAME, ApiErrorMessage.DUPLICATE_USERNAME);
            }

            user.UserName = userName;
            user.NormalizedUserName = userName.ToUpper();
        }

        user.DateOfBirth = req.DateOfBirth;
        user.Gender = req.Gender != null ? (int)req.Gender : null;
        user.Location = req.Location;
        user.PhoneNumber = req.PhoneNumber;

        await _context.SaveChangesAsync(default);

        await SyncWalletUserInfo(user);

        return await CreateUserRespone(user);
    }

    private async Task<UserProfileResponse> CreateUserRespone(User? user)
    {
        if (user == null)
        {
            return new UserProfileResponse();
        }
        var followingCount = await GetFollowingCountAsync(user.Id);
        var followersCount = await GetFollowerCountAsync(user.Id);
        return new UserProfileResponse
        {
            Id = user.Id,
            AvatarUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(user.Avatar),
            Email = user.Email,
            JoinDate = user.CreatedOn,
            ProfileName = user.ProfileName,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            CoverPhotoUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(user.CoverPhoto),
            Location = user.Location,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            EmailConfirmed = user.EmailConfirmed,
            ProfileId = user.ProfileId,
            PremiumDate = user.PremiumDate,
            LastLoginDate = user.LastLoginDate,
            IsPremium = user.IsPremium,
            NumberOfFollowing = followingCount,
            NumberOfFollowers = followersCount
        };
    }

    private async Task<UserProfileResponse> CreateUserResponeByUsername(User? user)
    {
        var currentUserId = _currentUserService.Session?.UserId;
        if (user == null)
        {
            return new UserProfileResponse();
        }
        var followingCount = await GetFollowingCountAsync(user.Id);
        var followersCount = await GetFollowerCountAsync(user.Id);
        return new UserProfileResponse
        {
            Id = user.Id,
            AvatarUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(user.Avatar),
            Email = user.Email,
            JoinDate = user.CreatedOn,
            ProfileName = user.ProfileName,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            PhoneNumber = user.PhoneNumber,
            CoverPhotoUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(user.CoverPhoto),
            Location = user.Location,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            EmailConfirmed = user.EmailConfirmed,
            ProfileId = user.ProfileId,
            PremiumDate = user.PremiumDate,
            LastLoginDate = user.LastLoginDate,
            IsPremium = user.IsPremium,
            NumberOfFollowing = followingCount,
            NumberOfFollowers = followersCount,
            IsFollowing = currentUserId == null ? false : await _context.UserFollowAvailable.AnyAsync(p => p.UserFollowerId == currentUserId && p.UserFollowingId == user.Id)
        };
    }

    private string GenerateNewFileName(string fileName)
    {
        return $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(fileName)}";
    }

    public async Task<List<SimilarProfile>> GetSimilarNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        name = "%" + name + "%";
        var profiles = await _userRepository.Connection.QueryAsync<SimilarProfile>(GetSimilarProfileName, new { Name = name });
        return profiles.ToList();
    }

    public async Task<List<SimilarProfilesMention>> GetSimilarProfilesMentionAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            var profileUsersRandom = await _userRepository.Connection.QueryAsync<SimilarProfilesMention>(GetRandomProfileNames, new { Name = name });
            foreach (var item in profileUsersRandom)
            {
                item.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.Avatar);
            }
            return profileUsersRandom.ToList();
        }

        name = "%" + name + "%";
        var profiles = await _userRepository.Connection.QueryAsync<SimilarProfilesMention>(GetSimilarProfileNamesMention, new { Name = name });
        foreach (var item in profiles)
        {
            item.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.Avatar);
        }
        return profiles.ToList();
    }

    private async Task SyncWalletUserInfo(User user)
    {
        //sync wallet profilename
        await _distributeManager.Deliver(new SyncDataDistributeItem
        {
            Data = new SyncData
            {
                TargetDb = SyncTargetDb.WALLETDB,
                TargetEntity = SyncTargetEntity.WALLET_USER_INFO,
                Data = new Dictionary<object, object>
                {
                    { user.Id, user}
                }
            }
        });
    }
    public async Task SyncWalletUserReward(Guid userId, float point, RewardType type)
    {
        //sync wallet profilename
        await _distributeManager.Deliver(new SyncDataDistributeItem
        {
            Data = new SyncData
            {
                TargetDb = SyncTargetDb.WALLETDB,
                TargetEntity = SyncTargetEntity.WALLET_USER_REWARD,
                Data = new Dictionary<object, object>
                {
                    { userId, new RewardSyncData {
                        Point = point,
                        Type = type
                    } }
                }
            }
        });
    }

    public async Task<PagedResponse<UserSearchResponse>> SearchUserbyKeyword(SmartLookupSearchUserR input)
    {
        PagedResponse<UserSearchResponse> results;
        if (string.IsNullOrWhiteSpace(input.ProfileName))
        {
            return new PagedResponse<UserSearchResponse>(0);
        }
        var keywords = input.ProfileName.ToLower().Split(' ');
        var query = $@"SELECT ""ProfileName"",
                                  ""ProfileId"",
                                  ""Avatar"",
                                  ""Id""
                          FROM identity.""Users"" 
                          [QueryCondition]
                          OFFSET @Offset 
                          LIMIT @PageSize;

                          SELECT COUNT(*) AS TotalItems 
                          FROM identity.""Users""
                          [QueryCondition]";
        bool first = true;
        var queryCondition = "";
        foreach (var word in keywords)
        {
            if (first)
            {
                queryCondition += $@"WHERE LOWER(""ProfileName"") LIKE '%{word}%'";
                first = false;
            }
            else
            {
                queryCondition += $@"OR LOWER(""ProfileName"") LIKE '%{word}%'";
            }
        }
        queryCondition += @"AND ""IsDelete"" = false";
        query = query.Replace("[QueryCondition]", queryCondition);
        var offset = input.PageSize * (input.PageNumber - 1);
        var multi = await _userRepository.Connection.QueryMultipleAsync(query, new
        {
            Offset = offset,
            PageSize = input.PageSize
        });
        var items = await multi.ReadAsync<UserSearchResponse>().ConfigureAwait(false);
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items.Any())
        {
            results = new PagedResponse<UserSearchResponse>(totalItems, input.PageNumber, input.PageSize);
            var userId = _currentUserService.Session?.UserId;
            bool isHaveUser = false;
            var userFollowingIds = new List<Guid>();
            if (userId != null)
            {
                userFollowingIds = await _context.UserFollowAvailable.AsNoTracking()
                                                                        .Where(p => p.UserFollowerId == userId)
                                                                        .Select(p => p.UserFollowingId)
                                                                        .ToListAsync();
                isHaveUser = userFollowingIds.Count > 0;
            }
            if (isHaveUser)
            {
                foreach (var item in items)
                {
                    item.IsFollowing = userFollowingIds.Contains(item.Id);
                    item.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.Avatar);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    item.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.Avatar);
                }
            }

            results.Items = items;
        }
        else
        {
            results = new PagedResponse<UserSearchResponse>(0);
        }
        return results;
    }

    public async Task<UserProfileAvatarResponse> GetUserAvatar(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return GetDefaultUser();
        }

        var user = await _userRepository.Connection.QueryFirstOrDefaultAsync<User>(GetUserAvatarById, new { UserId = userId });
        return new UserProfileAvatarResponse
        {
            AvatarUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(user.Avatar),
            ProfileId = user.ProfileId,
            ProfileName = user.ProfileName
        };
    }

    private UserProfileAvatarResponse GetDefaultUser()
    {
        string profileName = _configuration["DefaultUser:SystemProfileName"];
        string profileId = _configuration["DefaultUser:SystemProfileId"];
        string avatar = _configuration["DefaultUser:SystemAvatar"];

        return new UserProfileAvatarResponse
        {
            AvatarUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(avatar),
            ProfileId = profileId,
            ProfileName = profileName
        };
    }

    public async Task<List<UserFollowedResponse>> GetSuggestedProfilesNotFollowedAsync(string userName)
    {
        var currentIdProfileWatching = await _context.Users.AsNoTracking()
                                                    .Where(p => p.UserName == userName)
                                                    .Select(p => p.Id)
                                                    .FirstOrDefaultAsync();

        var userIdLoggedIn = _currentUserService.Session?.UserId;
        var result = new List<UserFollowedResponse>();

        if (userIdLoggedIn == null)
        {
            result = await _context.UserAvailable.AsNoTracking()
                 .Where(p => p.Id != currentIdProfileWatching)
                 .OrderBy(p => Guid.NewGuid())
                 .Select(p => new UserFollowedResponse
                 {
                     UserId = p.Id,
                     ProfileName = p.ProfileName,
                     Avatar = p.Avatar,
                     UserName = p.UserName,
                 })
                 .Take(5)
                 .ToListAsync();
        }

        else
        {
            var userId = await _context.Users.AsNoTracking()
                                            .Where(p => p.Id == userIdLoggedIn)
                                            .Select(p => p.Id)
                                            .FirstOrDefaultAsync();

            if (userId == null)
            {
                throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
            }

            var qUser = _context.UserAvailable;
            var qUserFollow = _context.UserFollowAvailable.Where(p => p.UserFollowerId == userId);

            result = await (from a in qUser
                            where !qUserFollow
                            .Select(p => p.UserFollowingId)
                            .Contains(a.Id) &&
                            a.Id != userId &&
                            a.Id != currentIdProfileWatching
                            orderby Guid.NewGuid()
                            select new UserFollowedResponse
                            {
                                UserId = a.Id,
                                ProfileName = a.ProfileName,
                                Avatar = a.Avatar,
                                UserName = a.UserName
                            })
                            .Take(5)
                            .ToListAsync();

            if (result.Any())
            {
                foreach (var i in result)
                {
                    i.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(i.Avatar + "");
                }
            }

        }
        return result;
    }

    public async Task<PagedResponse<UserFollowedResponse>> GetFollowingProfilesAsync(UserNamePagingR req)
    {
        var user = await _context.Users.AsNoTracking().Where(p => p.UserName == req.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        PagedResponse<UserFollowedResponse> res;
        var userIdWatchingProfile = _currentUserService.Session?.UserId;
        var userFollowingIds = new List<Guid>();
        bool isHaveUser = false;

        if (userIdWatchingProfile != null)
        {
            userFollowingIds = await _context.UserFollowAvailable.AsNoTracking()
                                                                    .Where(p => p.UserFollowerId == userIdWatchingProfile)
                                                                    .Select(p => p.UserFollowingId)
                                                                    .ToListAsync();
            isHaveUser = userFollowingIds.Count > 0;
        }

        var offset = req.PageSize * (req.PageNumber - 1);
        var qUser = _context.UserAvailable;
        var qUserFollow = _context.UserFollowAvailable.Where(p => p.UserFollowerId == user.Id);

        var userFollowing = from a in qUser
                            join b in qUserFollow
                              on a.Id equals b.UserFollowingId
                            where b.UserFollowerId == user.Id
                            select new UserFollowedResponse
                            {
                                UserId = a.Id,
                                ProfileName = a.ProfileName,
                                Avatar = a.Avatar,
                                UserName = a.UserName,
                                IsFollowing = isHaveUser ? userFollowingIds.Contains(a.Id) : false
                            };

        // Paging
        var totalItems = userFollowing.Count();
        var items = await userFollowing.Skip(offset).Take(req.PageSize).ToListAsync();

        // Update link MinIO
        foreach (var i in items)
        {
            i.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(i.Avatar + "");
        }
        if (totalItems > 0)
        {
            res = new PagedResponse<UserFollowedResponse>(totalItems, req.PageNumber, req.PageSize);
            res.Items = items;
        }
        else
        {
            res = new PagedResponse<UserFollowedResponse>(0);
        }
        return res;
    }

    public async Task<PagedResponse<UserFollowedResponse>> GetFollowedProfileAsync(UserNamePagingR req)
    {

        var user = await _context.Users.AsNoTracking().Where(p => p.UserName == req.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var userIdWatchingProfile = _currentUserService.Session?.UserId;
        var userFollowingIds = new List<Guid>();
        PagedResponse<UserFollowedResponse> res;
        bool isHaveUser = false;

        if (userIdWatchingProfile != null)
        {
            userFollowingIds = await _context.UserFollowAvailable.AsNoTracking()
                                                                    .Where(p => p.UserFollowerId == userIdWatchingProfile)
                                                                    .Select(p => p.UserFollowingId)
                                                                    .ToListAsync();
            isHaveUser = userFollowingIds.Count > 0;
        }

        var offset = req.PageSize * (req.PageNumber - 1);
        var qUser = _context.UserAvailable;
        var qUserFollow = _context.UserFollowAvailable.Where(p => p.UserFollowingId == user.Id);

        var userFollowed = from a in qUser
                           join b in qUserFollow
                             on a.Id equals b.UserFollowerId
                           where b.UserFollowingId == user.Id
                           select new UserFollowedResponse
                           {
                               UserId = a.Id,
                               ProfileName = a.ProfileName,
                               Avatar = a.Avatar,
                               UserName = a.UserName,
                           };


        // Paging
        var totalItems = userFollowed.Count();
        var items = await userFollowed.Skip(offset).Take(req.PageSize).ToListAsync();

        // Update link MinIO
        foreach (var i in items)
        {
            i.IsFollowing = isHaveUser ? userFollowingIds.Any(p => p == i.UserId) : false;
            i.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(i.Avatar + "");
        }
        if (totalItems > 0)
        {
            res = new PagedResponse<UserFollowedResponse>(totalItems, req.PageNumber, req.PageSize);
            res.Items = items;
        }
        else
        {
            res = new PagedResponse<UserFollowedResponse>(0);
        }
        return res;
    }

    public async Task<bool> FollowUserAsync(Guid userId)
    {
        var ss = _currentUserService.Session;
        if (ss == null || userId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var user = await _context.Users.FindAsync(ss.UserId);
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }
        if (ss.UserId == userId)
        {
            throw new BadRequestException(ApiErrorCode.INVALID_OPERATION, ApiErrorMessage.INVALID_OPERATION);
        }

        var qUserFollow = _context.UserFollows.Where(p => p.UserFollowerId == ss.UserId && p.UserFollowingId == userId);
        var userFollow = await qUserFollow.FirstOrDefaultAsync();
        if (userFollow != null)
        {
            if (!userFollow.IsDelete)
            {
                throw new BadRequestException(ApiErrorCode.ALREADY_EXISTS, ApiErrorMessage.ALREADY_EXISTS);
            }
            else
            {
                userFollow.IsDelete = false;
                userFollow.ModifiedOn = DateTime.UtcNow;
                userFollow.ModifiedBy = ss.UserId;
                _context.UserFollows.Update(userFollow);
                await _context.SaveChangesAsync(default);
                return true;
            }
        }
        userFollow = new UserFollow
        {
            UserFollowerId = ss.UserId,
            UserFollowingId = userId,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = ss.UserId,
            ModifiedOn = DateTime.UtcNow,
            ModifiedBy = ss.UserId,
            IsDelete = false
        };

        await _context.UserFollows.AddAsync(userFollow);
        await _context.SaveChangesAsync(default);

        return true;
    }

    public async Task<bool> UnFollowUserAsync(Guid userId)
    {
        var ss = _currentUserService.Session;
        if (ss == null || userId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }
        if (ss.UserId == userId)
        {
            throw new BadRequestException(ApiErrorCode.INVALID_OPERATION, ApiErrorMessage.INVALID_OPERATION);
        }

        var user = await _context.Users.FindAsync(ss.UserId);
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var qUserFollow = _context.UserFollows.Where(p => p.UserFollowerId == ss.UserId && p.UserFollowingId == userId);
        var userFollow = await qUserFollow.FirstOrDefaultAsync();

        if (userFollow == null || userFollow.IsDelete)
        {
            return false;
        }
        else
        {
            userFollow.IsDelete = true;
            userFollow.ModifiedOn = DateTime.UtcNow;
            userFollow.ModifiedBy = ss.UserId;
            _context.UserFollows.Update(userFollow);
            await _context.SaveChangesAsync(default);
            return false;
        }
    }

    private async Task<int> GetFollowerCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.INVALID_OPERATION, ApiErrorMessage.INVALID_OPERATION);
        }

        return await _context.UserFollowAvailable.CountAsync(p => p.UserFollowingId == userId);
    }

    private async Task<int> GetFollowingCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.INVALID_OPERATION, ApiErrorMessage.INVALID_OPERATION);
        }

        return await _context.UserFollowAvailable.CountAsync(p => p.UserFollowerId == userId);
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    #endregion
}

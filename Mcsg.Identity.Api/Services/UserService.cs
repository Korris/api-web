using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Lib.Common.Models;
using Lib.Common.Web;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Requests;
using Response;
using Validators.User;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;
using SettingCore = Common.Core.Constants.Setting;

public partial class UserService : IUserService
{
    private readonly ApplicationUserManager _userManager;
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISetting _setting;
    private readonly ISecurityAes _aes;
    private readonly IMcsgContext _context;
    private IConfiguration _configuration;
    private readonly IStorageClient _sc;
    private readonly ILogger<UserService> _logger;
    private readonly DistributeManager _distributeManager;

    public UserService(ApplicationUserManager userManager,
        IRepository<User> userRepository,
        ICurrentUserService currentUserService,
        ISetting setting,
        IMcsgContext context,
        IConfiguration configuration,
        IStorageClient sc,
        ILogger<UserService> logger,
        DistributeManager distributeManager
        )
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _userManager = userManager;
        _setting = setting;
        _aes = new SecurityAes(_setting.EncryptKey);
        _context = context;
        _configuration = configuration;
        _sc = sc;
        _logger = logger;
        _distributeManager = distributeManager;
    }

    public string GenerateReferralCode()
    {
        var referralCode = string.Empty;
        do
        {
            referralCode = 8.GetRandomString();
        } while (_userManager.UserAvailable.FirstOrDefault(x => x.ReferralCode == referralCode) != null);

        return referralCode;
    }

    public async Task<bool> ConfirmEmailAsync(string email)
    {
        var iResult = await _userRepository.Connection.ExecuteAsync(UpdateEmailConfirmedCommand, new
        {
            email,
            id = _currentUserService.Session.UserId
        });

        return iResult > 0;
    }

    public async Task<bool> ConfirmPhoneNumberAsync(string phone)
    {
        var iResult = await _userRepository.Connection.ExecuteAsync(UpdatePhoneNumberConfirmedCommand, new
        {
            phone,
            id = _currentUserService.Session.UserId
        });

        return iResult > 0;
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
        var user = await _userManager.UserAvailable.FirstOrDefaultAsync(p => p.UserName == userName);
        return await CreateUserResponeByUsername(user);
    }

    public async Task<PagedResponse<UserFollowedResponse>> GetFollowingProfilesAsync(UserNamePagingR req)
    {
        var user = await _context.UserAvailable.AsNoTracking().Where(p => p.UserName == req.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new BadRequestException(E119, M119);
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

    public async Task<bool> FollowUserAsync(Guid userId)
    {
        var ss = _currentUserService.Session;
        if (ss == null || userId == Guid.Empty)
        {
            throw new BadRequestException(E119, M119);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == ss.UserId);
        if (user == null)
        {
            throw new BadRequestException(E119, M119);
        }
        if (ss.UserId == userId)
        {
            throw new BadRequestException(E120, M120);
        }

        var qUserFollow = _context.UserFollows.Where(p => p.UserFollowerId == ss.UserId && p.UserFollowingId == userId);
        var userFollow = await qUserFollow.FirstOrDefaultAsync();
        if (userFollow != null)
        {
            if (!userFollow.IsDelete)
            {
                throw new BadRequestException(E121, M121);
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

    public async Task<List<UserFollowedResponse>> GetSuggestedProfilesNotFollowedAsync(string userName)
    {
        var currentIdProfileWatching = await _context.UserAvailable.AsNoTracking()
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
            var userId = await _context.UserAvailable.AsNoTracking()
                                            .Where(p => p.Id == userIdLoggedIn)
                                            .Select(p => p.Id)
                                            .FirstOrDefaultAsync();

            if (userId == null)
            {
                throw new BadRequestException(E119, M119);
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

    public async Task<bool> UnFollowUserAsync(Guid userId)
    {
        var ss = _currentUserService.Session;
        if (ss == null || userId == Guid.Empty)
        {
            throw new BadRequestException(E119, M119);
        }
        if (ss.UserId == userId)
        {
            throw new BadRequestException(E120, M120);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == ss.UserId);
        if (user == null)
        {
            throw new BadRequestException(E119, M119);
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
            throw new BadRequestException(E127, M127);
        }

        var ss = _currentUserService.Session;
        if (ss == null)
        {
            throw new BadRequestException(E119, M119);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == ss.UserId);
        if (user == null)
        {
            throw new BadRequestException(E119, M119);
        }

        if (user.ProfileName != profileName)
        {
            await _context.SmartLookups
                .Where(p => p.Keyword == user.ProfileName && p.KeywordType == LookupKeywordType.People)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Keyword, profileName));

            await _context.SmartLookupUsers
                .Where(p => p.Keyword == user.ProfileName && p.KeywordType == LookupKeywordType.People)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Keyword, profileName));

            user.ProfileName = profileName;
        }

        var userName = req.UserName?.Trim();
        if (!string.IsNullOrWhiteSpace(userName))
        {
            var userNameHistory = await _context.UserNameHistories.FirstOrDefaultAsync(p => p.UserName == userName);
            if (userNameHistory == null)
            {
                if (!user.IsPremium && userName.Length < Common.SeedWork.Constants.Validator.UserNameFree.Min)
                {
                    throw new BadRequestException(E124, M124);
                }

                userNameHistory = new UserNameHistory
                {
                    UserId = user.Id,
                    UserName = userName,
                    CreatedBy = user.Id
                };

                await _context.UserNameHistories.AddAsync(userNameHistory);
                var postToUpdate = await _context.SocialPosts.ToListAsync();

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
                throw new BadRequestException(E125, M125);
            }

            user.UserName = userName;
            user.NormalizedUserName = userName.ToUpper();
        }

        user.DateOfBirth = req.DateOfBirth;
        user.Gender = req.Gender != null ? (int)req.Gender : null;
        user.Location = req.Location;
        user.PhoneNumber = req.PhoneNumber;
        user.IsWalletShowing = req.IsWalletShowing;

        await _context.SaveChangesAsync(default);

        await SyncWalletUserInfo(user);

        return await CreateUserRespone(user);
    }

    public async Task<UserProfileAvatarResponse?> GetUserAvatar(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return GetDefaultUser();
        }

        return await _userManager.UserAvailable.AsNoTracking()
            .Where(p => p.Id == userId)
            .Select(p => new UserProfileAvatarResponse
            {
                AvatarUrl = _setting.Minio.MediaApiUrl.ToPublicImageUrl(p.Avatar),
                ProfileId = p.ProfileId,
                ProfileName = p.ProfileName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateR request)
    {
        var file = request?.Avatar;
        if (request == null || file == null)
        {
            throw new BadRequestException(E122, M122);
        }

        var fileExtension = Path.GetExtension(file.FileName);
        if (!SettingCore.FileExt.Images.Any(p => p == fileExtension.ToLower()))
        {
            throw new BadRequestException(E123, M123);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == request.UserId);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var bucketName = _sc.Strategy.BucketNamePublic;
        var type = string.IsNullOrWhiteSpace(request.Type) ? "" : $"/{request.Type}".ToPlural();
        var objectName = $"{SettingCore.MinioFolder.User}/{user.UserFolder}{type}/{file.FileName}";

        var fileName = string.Empty;
        try
        {
            var isExistFile = await _sc.Strategy.StatObjectAsync(objectName, bucketName);
            if (isExistFile != null)
            {
                fileName = GenerateNewFileName(file.FileName);
            }
            else
            {
                fileName = file.FileName;
            }
            user.Avatar = fileName;

            Stream? newFormFile = null;
            using (var imageContent = file.OpenReadStream())
            {
                newFormFile = imageContent.ResizeImage(180, 180);
            }

            await _sc.Strategy.PutObject(newFormFile, objectName, bucketName);
            newFormFile.Close();

            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(UpdateUserAvatar), request);
            throw new BadRequestException(E500, ex.Message);
        }

        return new UserAvatarUpdateResponse { Avatar = _setting.Minio.GetPublicUrl(bucketName, objectName) };
    }

    public async Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateR request)
    {
        var file = request?.CoverPhoto;
        if (request == null || file == null)
        {
            throw new BadRequestException(E122, M122);
        }

        var fileExtension = Path.GetExtension(file.FileName);
        if (!SettingCore.FileExt.Images.Any(p => p == fileExtension.ToLower()))
        {
            throw new BadRequestException(E123, M123);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == request.UserId);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var bucketName = _sc.Strategy.BucketNamePublic;
        var type = string.IsNullOrWhiteSpace(request.Type) ? "" : $"/{request.Type}".ToPlural();
        var objectName = $"{SettingCore.MinioFolder.User}/{user.UserFolder}{type}/{file.FileName}";

        var fileName = string.Empty;
        try
        {
            var isExistFile = await _sc.Strategy.StatObjectAsync(objectName, bucketName);
            if (isExistFile != null)
            {
                fileName = GenerateNewFileName(file.FileName);
            }
            else
            {
                fileName = file.FileName;
            }
            user.CoverPhoto = fileName;

            await _sc.Strategy.PutObject(file.OpenReadStream(), objectName, bucketName);

            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(UpdateUserAvatar), request);
            throw new BadRequestException(E500, ex.Message);
        }

        return new UserCoverPhotoUpdateResponse { CoverPhoto = _setting.Minio.GetPublicUrl(bucketName, objectName) };
    }

    public async Task<PagedResponse<UserFollowedResponse>> GetFollowedProfileAsync(UserNamePagingR req)
    {

        var user = await _context.UserAvailable.AsNoTracking().Where(p => p.UserName == req.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new BadRequestException(E119, M119);
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

    private string GenerateNewFileName(string fileName)
    {
        return $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}{Path.GetExtension(fileName)}";
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
            Email = _aes.DecryptText(user.Email) + "",
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
            IsFollowing = currentUserId == null ? false : await _context.UserFollowAvailable.AnyAsync(p => p.UserFollowerId == currentUserId && p.UserFollowingId == user.Id),
            IsWalletShowing = user.IsWalletShowing
        };
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
            Email = _aes.DecryptText(user.Email) + "",
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
            IsWalletShowing = user.IsWalletShowing,
            ReferralCode = user.ReferralCode,
        };
    }

    private async Task<int> GetFollowerCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException(E120, M120);
        }

        return await _context.UserFollowAvailable.CountAsync(p => p.UserFollowingId == userId);
    }

    private async Task<int> GetFollowingCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException(E120, M120);
        }

        return await _context.UserFollowAvailable.CountAsync(p => p.UserFollowerId == userId);
    }
}

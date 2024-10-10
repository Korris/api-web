using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services;

using Common.Core.Constants;
using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.Enums;
using Common.SeedWork;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Lib.Common.Models;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Requests;
using Response;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;
using SettingCore = Common.Core.Constants.Setting;

public partial class UserService : BaseMinioS, IUserService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="userManager"></param>
    /// <param name="userRepository"></param>
    /// <param name="currentUserService"></param>
    /// <param name="configuration"></param>
    /// <param name="distributeManager"></param>
    public UserService(IMcsgContext context, ISetting setting, IStorageClient sc, ApplicationUserManager userManager, IRepository<User> userRepository, ICurrentUserService currentUserService, IConfiguration configuration, DistributeManager distributeManager) : base(context, setting, sc)
    {
        _aes = new SecurityAes(_setting.EncryptKey);

        _userManager = userManager;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _configuration = configuration;
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

    public async Task<User.FullProfileDto> GetCurrentUserAsync()
    {
        var ss = _currentUserService.Session;
        if (ss == null)
        {
            throw new BadRequestException(E119, M119);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == ss.UserId);
        var res = await CreateUserRespone(user, ss.Roles, true);

        // Check first login
        if (user != null && user.LastLoginDate == null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(default);
        }

        return res;
    }

    public async Task<User.FullProfileDto> GetUserByUserNameAsync(string userName)
    {
        var user = await _userManager.UserAvailable.FirstOrDefaultAsync(p => p.UserName == userName);
        if (user == null)
        {
            return new User.FullProfileDto();
        }

        var roles = await _userManager.GetRolesAsync(user);
        return await CreateUserRespone(user, string.Join(",", roles), false);
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
            return profileUsersRandom.ToList();
        }

        name = "%" + name + "%";
        var profiles = await _userRepository.Connection.QueryAsync<SimilarProfilesMention>(GetSimilarProfileNamesMention, new { Name = name });

        return profiles.ToList();
    }

    public async Task<User.FullProfileDto> UpdateUserProfile(UserProfileUpdateR req)
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

        user.DateOfBirth = req.DateOfBirth;
        user.Gender = (int)(req.Gender + "").ToEnum(GenderType.Other);
        user.Location = req.Location;
        user.PhoneNumber = req.PhoneNumber;
        user.IsWalletShowing = req.IsWalletShowing;

        await _context.SaveChangesAsync(default);

        await SyncWalletUserInfo(user);

        return await CreateUserRespone(user, ss.Roles, true);
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
                AvatarUrl = p.Avatar,
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

        var bucketName = _sc.GetStrategy(request.MinioInstance).BucketNamePublic;
        var type = string.IsNullOrWhiteSpace(request.Type) ? "" : $"/{request.Type}".ToPlural();
        var hashId = Setting.ResourceConfig.HashLength.GetRandomString();
        var hashFileName = file.GetHashName(hashId);
        var objectName = $"{SettingCore.MinioFolder.User}/{user.UserFolder}{type}/{hashFileName}";

        try
        {
            var fs = file.OpenReadStream().ResizeImage(500, 500, 85);
            if (fs != null)
            {
                await _sc.GetStrategy(request.MinioInstance).PutObject(fs, objectName, bucketName);
                fs.Close();

                user.Avatar = _setting.GetMinio(request.MinioInstance).GetPublicUrl(bucketName, objectName);
                await _context.SaveChangesAsync(default);
            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(E500, ex.Message);
        }

        return new UserAvatarUpdateResponse { Avatar = user.Avatar };
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

        var bucketName = _sc.GetStrategy(request.MinioInstance).BucketNamePublic;
        var type = string.IsNullOrWhiteSpace(request.Type) ? "" : $"/{request.Type}".ToPlural();
        var hashId = Setting.ResourceConfig.HashLength.GetRandomString();
        var hashFileName = file.GetHashName(hashId);
        var objectName = $"{SettingCore.MinioFolder.User}/{user.UserFolder}{type}/{hashFileName}";

        try
        {
            await _sc.GetStrategy(request.MinioInstance).PutObject(file.OpenReadStream(), objectName, bucketName);
            user.CoverPhoto = _setting.GetMinio(request.MinioInstance).GetPublicUrl(bucketName, objectName);
            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(E500, ex.Message);
        }

        return new UserCoverPhotoUpdateResponse { CoverPhoto = user.CoverPhoto };
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
            i.Avatar = i.Avatar;
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
            AvatarUrl = avatar,
            ProfileId = profileId,
            ProfileName = profileName
        };
    }

    private async Task<User.FullProfileDto> CreateUserRespone(User? user, string? roles, bool decryptEmail)
    {
        var currentUserId = _currentUserService.Session?.UserId;
        if (user == null)
        {
            return new User.FullProfileDto();
        }

        var res = user.ToFullProfileDto(roles);
        res.NumberOfFollowing = await GetFollowingCountAsync(user.Id);
        res.NumberOfFollowers = await GetFollowerCountAsync(user.Id);
        res.IsFollowing = currentUserId != null && await _context.UserFollowAvailable.AnyAsync(p => p.UserFollowerId == currentUserId && p.UserFollowingId == user.Id);

        if (decryptEmail)
        {
            res.Email = _aes.DecryptText(user.Email);
        }
        else
        {
            res.Email = null;
        }

        return res;
    }

    private async Task<int> GetFollowerCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException(E120, M120);
        }

        return await (from a in _context.UserFollowAvailable
                      join b in _context.Users on a.UserFollowerId equals b.Id
                      where a.UserFollowingId == userId && b.IsDelete == false
                      select a).CountAsync();
    }

    private async Task<int> GetFollowingCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException(E120, M120);
        }

        return await (from a in _context.UserFollowAvailable
                      join b in _context.Users on a.UserFollowingId equals b.Id
                      where a.UserFollowerId == userId && b.IsDelete == false
                      select a).CountAsync();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    private readonly ApplicationUserManager _userManager;
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    private readonly DistributeManager _distributeManager;

    #endregion
}

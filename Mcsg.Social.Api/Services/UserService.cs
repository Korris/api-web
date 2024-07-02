using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services
{
    using Common.Core.Extensions;
    using Common.Core.Interfaces;
    using Common.SeedWork.Constants;
    using Constants;
    using Interfaces;
    using Lib.Common.Constants;
    using Lib.Common.Distributor;
    using Lib.Common.Enums;
    using Lib.Common.Exceptions;
    using Lib.Common.Helpers;
    using Lib.Common.Models;
    using Lib.Common.Web.Security;
    using Lib.Data;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Entities.Common;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Models;
    using Requests;
    using Validators;
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
            McsgDbContext context,
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
            var user = await _userRepository.GetByIdAsync(_currentUserService.Session.UserId);
            var userRespone = CreateUserRespone(user);

            //Check first login
            if (user.LastLoginDate == null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }

            return userRespone;
        }
        public async Task<UserProfileResponse> GetUserByUserNameAsync(string profileName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(p => p.ProfileName == profileName);
            return CreateUserRespone(user);
        }

        public async Task<UserAvatarUpdateResponse> UpdateUserAvatar(UserAvatarUpdateRequest userAvatarUpdateRequest)
        {
            if (userAvatarUpdateRequest?.Avatar == null)
                throw new BadRequestException(ApiErrorCode.INVALID_AVATAR_IMAGE, ApiErrorMessage.INVALID_AVATAR_IMAGE);

            var fileExtension = Path.GetExtension(userAvatarUpdateRequest.Avatar.FileName);

            if (!FileExt.Images.Any(ext => ext == fileExtension.ToLower()))
                throw new BadRequestException(ApiErrorCode.INVALID_FILE_TYPE, ApiErrorMessage.INVALID_AVATAR_FILE_TYPE);

            string fileName = string.Empty;
            try
            {
                var user = await _userRepository.GetByIdAsync(_currentUserService.Session.UserId);

                var objectName = $"{BlobStorageDefinition.ImageContainer}/{userAvatarUpdateRequest.Avatar.FileName}";
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

                objectName = $"{BlobStorageDefinition.ImageContainer}/{fileName}";
                await _sc.Strategy.PutObject(newFormFile, objectName, null);
                newFormFile.Close();

                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateUserAvatar), userAvatarUpdateRequest);
                throw new BadRequestException(ErrorCodes.ApiErrorCode, ex.Message);
            }

            return new UserAvatarUpdateResponse() { Avatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, fileName) };
        }

        public async Task<UserCoverPhotoUpdateResponse> UpdateUserCoverPhoto(UserCoverPhotoUpdateRequest userCoverPhotoUpdateRequest)
        {
            if (userCoverPhotoUpdateRequest?.CoverPhoto == null)
                throw new BadRequestException(ApiErrorCode.INVALID_COVER_PHOTO_IMAGE, ApiErrorMessage.INVALID_COVER_PHOTO_IMAGE);

            var fileExtension = Path.GetExtension(userCoverPhotoUpdateRequest.CoverPhoto.FileName);
            if (!FileExt.Images.Any(ext => ext == fileExtension.ToLower()))
                throw new BadRequestException(ApiErrorCode.INVALID_FILE_TYPE, ApiErrorMessage.INVALID_COVER_PHOTO_FILE_TYPE);

            string fileName = string.Empty;
            try
            {
                var user = await _userRepository.GetByIdAsync(_currentUserService.Session.UserId);
                var objectName = $"{BlobStorageDefinition.ImageContainer}/{userCoverPhotoUpdateRequest.CoverPhoto.FileName}";
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
                objectName = $"{BlobStorageDefinition.ImageContainer}/{fileName}";
                await _sc.Strategy.PutObject(userCoverPhotoUpdateRequest.CoverPhoto.OpenReadStream(), objectName, null);
                await _userRepository.UpdateAsync(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateUserCoverPhoto), userCoverPhotoUpdateRequest);
                throw new BadRequestException(ErrorCodes.ApiErrorCode, ex.Message);
            }

            return new UserCoverPhotoUpdateResponse() { CoverPhoto = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, fileName) };
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
                user.ProfileId = profileName.Replace(" ", "-");
            }

            var userName = req.UserName?.Trim();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userNameHistory = await _context.UserNameHistories.FirstOrDefaultAsync(p => p.UserName == userName);
                if (userNameHistory == null)
                {
                    if (!user.IsPremium && userName.Length <= Validator.UserNamePremium.Max)
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

            await _context.SaveChangesAsync();

            await SyncWalletUserInfo(user);

            return CreateUserRespone(user);
        }

        private UserProfileResponse CreateUserRespone(User? user)
        {
            if (user == null)
            {
                return new UserProfileResponse();
            }

            return new UserProfileResponse
            {
                Id = user.Id,
                AvatarUrl = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, user.Avatar),
                Email = user.Email,
                JoinDate = user.CreatedDate,
                ProfileName = user.ProfileName,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                CoverPhotoUrl = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, user.CoverPhoto),
                Location = user.Location,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                EmailConfirmed = user.EmailConfirmed,
                ProfileId = user.ProfileId,
                PremiumDate = user.PremiumDate,
                LastLoginDate = user.LastLoginDate,
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
                    item.Avatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.Avatar);
                }
                return profileUsersRandom.ToList();
            }

            name = "%" + name + "%";
            var profiles = await _userRepository.Connection.QueryAsync<SimilarProfilesMention>(GetSimilarProfileNamesMention, new { Name = name });
            foreach (var item in profiles)
            {
                item.Avatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.Avatar);
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

        public async Task<PagedResults<UserSearchResponse>> SearchUserbyKeyword(SearchUserReq input)
        {
            PagedResults<UserSearchResponse> results;
            if (string.IsNullOrWhiteSpace(input.ProfileName))
            {
                return new PagedResults<UserSearchResponse>(0);
            }
            var keywords = input.ProfileName.ToLower().Split(' ');
            var query = $@"SELECT ""ProfileName"",
                                  ""ProfileId"",
                                  ""Avatar""
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
                results = new PagedResults<UserSearchResponse>(totalItems, input.PageNumber, input.PageSize);

                foreach (var item in items)
                {
                    item.Avatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.Avatar);
                }
                results.Items = items;
            }
            else
            {
                results = new PagedResults<UserSearchResponse>(0);
            }
            return results;
        }

        public async Task<UserProfileAvatarResponse> GetUserAvatar(Guid userId)
        {
            if (userId == Guid.Empty) return GetDefaultUser();
            var user = await _userRepository.Connection.QueryFirstOrDefaultAsync<User>(GetUserAvatarById, new { UserId = userId });
            return new UserProfileAvatarResponse
            {
                AvatarUrl = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, user.Avatar),
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
                AvatarUrl = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, avatar),
                ProfileId = profileId,
                ProfileName = profileName
            };
        }

        #region -- Fields --

        /// <summary>
        /// DB Context
        /// </summary>
        private readonly McsgDbContext _context;

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
}

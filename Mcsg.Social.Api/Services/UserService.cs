using Dapper;
using Mcsg.Social.Api.Constants;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Enums;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services
{
    public partial class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<SmartLookup> _smartLookupRepository;
        private readonly ICurrentUserService _currentUserService;
        private IConfiguration _configuration;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        private readonly DistributeManager _distributeManager;
        private readonly ILogger<UserService> _logger;

        public UserService(IRepository<User> userRepository,
            ICurrentUserService currentUserService,
            IConfiguration configuration,
            IRepository<SmartLookup> smartLookupRepository,
            DistributeManager distributeManager,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _configuration = configuration;
            _azureBlobStorageService = new AzureBlobStorageService(configuration["AzureBlobStoragePublic"]);
            _logger = logger;
            _smartLookupRepository = smartLookupRepository;
            _distributeManager = distributeManager;
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
            var user = await _userRepository.Connection.QueryFirstOrDefaultAsync<User>(GetUserProfileByName, new { ProfileName = profileName });
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

                var isExistFile = await _azureBlobStorageService.IsExistAsync(userAvatarUpdateRequest.Avatar.FileName, BlobStorageDefinition.CONST_BLOB_STORAGE_CONTAINER_NAME);
                if (isExistFile)
                {
                    fileName = GenerateNewFileName(userAvatarUpdateRequest.Avatar.FileName);
                }
                else
                {
                    fileName = userAvatarUpdateRequest.Avatar.FileName;
                }

                user.Avatar = fileName;

                MemoryStream newFormFile = null;
                using (var imageContent = userAvatarUpdateRequest.Avatar.OpenReadStream())
                {
                    newFormFile = ImageHelper.ResizeImage(imageContent, 180, 180);
                }

                await _azureBlobStorageService.UploadAsync(fileName, newFormFile, BlobStorageDefinition.CONST_BLOB_STORAGE_CONTAINER_NAME);
                newFormFile.Close();

                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateUserAvatar), userAvatarUpdateRequest);
                throw new BadRequestException(ErrorCodes.ApiErrorCode, ex.Message);
            }

            return new UserAvatarUpdateResponse() { Avatar = UrlHelper.GetPublicImageUrl(_configuration, fileName) };
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
                var isExistFile = await _azureBlobStorageService.IsExistAsync(userCoverPhotoUpdateRequest.CoverPhoto.FileName, BlobStorageDefinition.CONST_BLOB_STORAGE_CONTAINER_NAME);
                if (isExistFile)
                {
                    fileName = GenerateNewFileName(userCoverPhotoUpdateRequest.CoverPhoto.FileName);
                }
                else
                {
                    fileName = userCoverPhotoUpdateRequest.CoverPhoto.FileName;
                }
                user.CoverPhoto = fileName;
                await _azureBlobStorageService.UploadAsync(fileName, userCoverPhotoUpdateRequest.CoverPhoto.OpenReadStream(), BlobStorageDefinition.CONST_BLOB_STORAGE_CONTAINER_NAME);
                await _userRepository.UpdateAsync(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateUserCoverPhoto), userCoverPhotoUpdateRequest);
                throw new BadRequestException(ErrorCodes.ApiErrorCode, ex.Message);
            }

            return new UserCoverPhotoUpdateResponse() { CoverPhoto = UrlHelper.GetPublicImageUrl(_configuration, fileName) };
        }

        /// <summary>
        /// Only change email or phone number.
        /// Change both of them --> map to case change email and don't change the phone number.
        /// </summary>
        /// <param name="userProfileUpdateRequest"></param>
        /// <returns></returns>
        public async Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdateRequest userProfileUpdateRequest)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.Session.UserId);
            var profileName = userProfileUpdateRequest.ProfileName?.Trim();
            if (string.IsNullOrWhiteSpace(profileName))
                throw new BadRequestException(ApiErrorCode.PROFILE_NAME_NOT_EMPTY, ApiErrorMessage.PROFILE_NAME_NOT_EMPTY);

            if (!string.Equals(user.ProfileName, profileName))
            {
                var profiles = await _userRepository.Connection.QueryAsync<SimilarProfile>(CheckExistProfileName, new { Name = profileName });
                if (profiles.Any())
                {
                    throw new BadRequestException(ApiErrorCode.EXISTING_PROFILE_NAME, ApiErrorMessage.EXISTING_PROFILE_NAME);
                }

                await _smartLookupRepository.Connection.ExecuteAsync(UpdateSmartLookupProfileName, new
                {
                    newKeyword = profileName,
                    oldKeyword = user.ProfileName,
                });
                user.ProfileName = profileName;
                user.ProfileId = profileName.Replace(" ", "-");

            }

            user.DateOfBirth = userProfileUpdateRequest.DateOfBirth;
            user.Gender = userProfileUpdateRequest.Gender != null ? (int)userProfileUpdateRequest.Gender : null;
            user.Location = userProfileUpdateRequest.Location;
            user.PhoneNumber = userProfileUpdateRequest.PhoneNumber;
            await _userRepository.UpdateAsync(user);
            await SyncWalletUserInfo(user);
            return CreateUserRespone(user);
        }

        private UserProfileResponse CreateUserRespone(User user)
        {
            return new UserProfileResponse
            {
                Id = user.Id,
                AvatarUrl = UrlHelper.GetPublicImageUrl(_configuration, user.Avatar),
                Email = user.Email,
                JoinDate = user.CreatedDate,
                ProfileName = user.ProfileName,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                CoverPhotoUrl = UrlHelper.GetPublicImageUrl(_configuration, user.CoverPhoto),
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

        public async Task<UserProfileAvatarResponse> GetUserAvatar(Guid userId)
        {
            if (userId == Guid.Empty) return GetDefaultUser();
            var user = await _userRepository.Connection.QueryFirstOrDefaultAsync<User>(GetUserAvatarById, new { UserId = userId });
            return new UserProfileAvatarResponse
            {
                AvatarUrl = UrlHelper.GetPublicImageUrl(_configuration, user.Avatar),
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
                AvatarUrl = UrlHelper.GetPublicImageUrl(_configuration, avatar),
                ProfileId = profileId,
                ProfileName = profileName
            };
        }
    }
}

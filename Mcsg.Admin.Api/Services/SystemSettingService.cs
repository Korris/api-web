using Dapper;
using Mcsg.Admin.Api.Constants;
using Mcsg.Admin.Api.DTOs.Settings;
using Mcsg.Admin.Api.Services.Interface;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Constants;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using System.Text.Json;
using ComnonConstant = Mcsg.Lib.Common.Constants.ErrorCodes;

namespace Mcsg.Admin.Api.Services
{
    public partial class SystemSettingService : ISystemSettingService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SystemSetting> _systemSettingRepo;
        private readonly IRepository<SystemSettingHistory> _systemSettingHistoryRepo;
        private readonly ILogger<SystemSettingService> _logger;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        private readonly IConfiguration _configuration;

        public SystemSettingService(IUnitOfWork unitOfWork
            , ILogger<SystemSettingService> logger
            , ICurrentUserService currentUserService
            , IAzureBlobStorageService azureBlobStorageService
            , IConfiguration configuration)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _azureBlobStorageService = azureBlobStorageService;
            _systemSettingRepo = unitOfWork.GetRepository<SystemSetting>();

            _systemSettingHistoryRepo = unitOfWork.GetRepository<SystemSettingHistory>();

            _systemSettingHistoryRepo.TableName = $"{DbSchema.Default}\"{DbSchema.SystemSettingHistories_Table}\"";
        }

        public async Task<GlobalSettingRespone> GetGlobalSettingAsync()
        {
            var systemSetting = await _systemSettingRepo.Connection.QueryFirstOrDefaultAsync<SystemSetting>(ActiveSystemSettingQuery, new { Key = SystemSettings.CONST_GLOBAL_SETTING, IsActive = true });
            GlobalSettingRespone value = null;
            if (systemSetting != null)
            {
                value = JsonSerializer.Deserialize<GlobalSettingRespone>(systemSetting.Value);
                value.Id = systemSetting.Id;
            }
            if (!string.IsNullOrEmpty(value.Favicon))
                value.Favicon = string.Format(Path.Combine(_configuration["MediaUrl"], MediaConfig.PublicImageUrlPath), value.Favicon);
            return value;
        }

        public async Task<GlobalSettingRespone> UpdateGlobalSettingAsync(Guid id, GlobalSettingRequest request)
        {
            var systemSetting = await _systemSettingRepo.Connection.QueryFirstOrDefaultAsync<SystemSetting>(
                ActiveSystemSettingQuery, new { Key = SystemSettings.CONST_GLOBAL_SETTING, IsActive = true }) ?? throw new BadRequestException(ComnonConstant.QueryNotFound, AdminErrorMessages.GlobalSettingDoesNotExist);

            if (id.ToString() != systemSetting.Id.ToString())
            {
                throw new BadRequestException(AdminErrorCodes.GlobalSettingValidate,
                    string.Format(AdminErrorMessages.InValidGlobalSettingId, id.ToString()));
            }

            var newValue = JsonSerializer.Serialize(
                    new
                    {
                        request.Title,
                        Favicon = request.Favicon.FileName,
                        request.Description
                    });

            var systemSettingHistory = new SystemSettingHistory()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                OldValue = systemSetting.Value,
                NewValue = newValue
            };

            systemSetting.Value = newValue;

            systemSettingHistory.SystemSettingId = systemSetting.Id;
            systemSettingHistory.UserId = _currentUserService.Session.UserId;

            try
            {
                var isExistFile = await _azureBlobStorageService.IsExistAsync(request.Favicon.FileName, SystemSettings.CONST_BLOB_STORAGE_CONTAINER_NAME);
                if (isExistFile)
                {
                    await _azureBlobStorageService.DeleteAsync(request.Favicon.FileName, SystemSettings.CONST_BLOB_STORAGE_CONTAINER_NAME);
                }
                await _azureBlobStorageService.UploadAsync(request.Favicon.FileName, request.Favicon.OpenReadStream(), SystemSettings.CONST_BLOB_STORAGE_CONTAINER_NAME);

                await _systemSettingRepo.UpdateAsync(systemSetting);
                if (systemSettingHistory.OldValue != newValue)
                {
                    await _systemSettingHistoryRepo.InsertAsync(systemSettingHistory);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateGlobalSettingAsync), request);
                throw;
            }

            var result = new GlobalSettingRespone()
            {
                Id = systemSetting.Id,
                Title = request.Title,
                Favicon = request.Favicon.FileName,
                Description = request.Description
            };

            return result;
        }

        public async Task<EmailSettingRespone> GetEmailSettingAsync()
        {
            var systemSetting = await _systemSettingRepo.Connection.QueryFirstOrDefaultAsync<SystemSetting>(ActiveSystemSettingQuery, new { Key = SystemSettings.CONST_EMAIL_SETTING, IsActive = true });
            EmailSettingRespone value = null;
            if (systemSetting != null)
            {
                value = JsonSerializer.Deserialize<EmailSettingRespone>(systemSetting.Value);
                value.Password = CryptoHelper.Decrypt(value.Password, _configuration[SystemSettings.CONST_EMAIL_ENCRYPTION_KEY]);
                value.Id = systemSetting.Id;
            }

            return value;
        }

        public async Task<EmailSettingRespone> UpdateEmailSettingAsync(Guid id, EmailSettingRequest request)
        {
            var passwordHash = CryptoHelper.Encrypt(request.Password, _configuration[SystemSettings.CONST_EMAIL_ENCRYPTION_KEY]);

            var systemSetting = await _systemSettingRepo.Connection.QueryFirstOrDefaultAsync<SystemSetting>(
                ActiveSystemSettingQuery, new { Key = SystemSettings.CONST_EMAIL_SETTING, IsActive = true }) ?? throw new BadRequestException(ComnonConstant.QueryNotFound, AdminErrorMessages.EmailSettingDoesNotExist);

            if (id.ToString() != systemSetting.Id.ToString())
            {
                throw new BadRequestException(AdminErrorCodes.EmailSettingValidate,
                    string.Format(AdminErrorMessages.InValidEmailSettingId, id.ToString()));
            }

            var newValue = JsonSerializer.Serialize(
                    new
                    {
                        Host = request.Host,
                        Port = request.Port,
                        Email = request.Email,
                        Password = passwordHash,
                        DisplayName = request.DisplayName
                    });

            var systemSettingHistory = new SystemSettingHistory()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                OldValue = systemSetting.Value,
                NewValue = newValue
            };

            systemSetting.Value = newValue;

            systemSettingHistory.SystemSettingId = systemSetting.Id;
            systemSettingHistory.UserId = _currentUserService.Session.UserId;

            try
            {
                await _systemSettingRepo.UpdateAsync(systemSetting);
                if (systemSettingHistory.OldValue != newValue)
                {
                    await _systemSettingHistoryRepo.InsertAsync(systemSettingHistory);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateEmailSettingAsync), request);
                throw new Exception(AdminErrorMessages.UpdateEmailSettingExceptionMessage);
            }

            var result = new EmailSettingRespone()
            {
                Id = systemSetting.Id,
                Host = request.Host,
                Port = request.Port,
                Email = request.Email,
                Password = request.Password,
                DisplayName = request.DisplayName
            };

            return result;
        }
    }
}

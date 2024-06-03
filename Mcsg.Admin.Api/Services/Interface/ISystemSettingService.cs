using Mcsg.Admin.Api.DTOs.Settings;

namespace Mcsg.Admin.Api.Services.Interface
{
    public interface ISystemSettingService
    {
        Task<GlobalSettingRespone> GetGlobalSettingAsync();
        Task<GlobalSettingRespone> UpdateGlobalSettingAsync(Guid id, GlobalSettingRequest globalSettingRequest);
        Task<EmailSettingRespone> GetEmailSettingAsync();
        Task<EmailSettingRespone> UpdateEmailSettingAsync(Guid id, EmailSettingRequest globalSettingRequest);
    }
}

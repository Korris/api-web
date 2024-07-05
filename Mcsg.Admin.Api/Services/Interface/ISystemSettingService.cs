namespace Mcsg.Admin.Api.Services.Interface
{
    using Dtos;
    using Requests;

    public interface ISystemSettingService
    {
        Task<GlobalSettingRespone> GetGlobalSettingAsync();
        Task<GlobalSettingRespone> UpdateGlobalSettingAsync(Guid id, GlobalSettingRequest globalSettingRequest);
        Task<EmailSettingRespone> GetEmailSettingAsync();
        Task<EmailSettingRespone> UpdateEmailSettingAsync(Guid id, EmailSettingRequest globalSettingRequest);
    }
}

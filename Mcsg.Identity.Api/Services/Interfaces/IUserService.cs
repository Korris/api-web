namespace Mcsg.Identity.Api.Services.Interface
{
    public interface IUserService
    {
        string GenerateReferralCode();
        string GenerateUserName(string email, string phone);
        Task<string> GenerateProfileName(string email, string phone);
        Task<bool> ConfirmEmailAsync(string email);
        Task<bool> ConfirmPhoneNumberAsync(string phone);
    }
}

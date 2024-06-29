namespace Mcsg.Identity.Api.Services.Interface
{
    public interface IUserService
    {
        string GenerateReferralCode();
        Task<bool> ConfirmEmailAsync(string email);
        Task<bool> ConfirmPhoneNumberAsync(string phone);
    }
}

namespace Mcsg.Identity.Api.Interfaces
{
    public interface IUserService
    {
        string GenerateReferralCode();
        Task<bool> ConfirmEmailAsync(string email);
        Task<bool> ConfirmPhoneNumberAsync(string phone);
    }
}

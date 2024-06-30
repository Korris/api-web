namespace Mcsg.Identity.Api.Interfaces
{
    using Lib.Data.Enums;
    using Requests;
    using Response;

    public interface IAuthenticationService
    {
        Task<VerifyUserResponse> RegisterUser(RegisterUserReq request);
        Task<VerifyUserResponse> ResendOtp(UserOtpType type, string otpToken = "");
        Task<TokenResponse> LoginUser(LoginUserReq request);
        Task<bool> LogOut();
        Task<TokenResponse> SocialLogin(string socialType, string socialToken);
        Task<VerifyUserResponse> ForgotPassword(string email, string phone);
        Task<bool> ResetPassword(ResetPasswordReq request);
        Task<TokenResponse> SetUserPassword(string password, string confirmPassword);
        Task<bool> CreateNewUserPassword(string email, string phone, string otp, string otpToken, string password, string confirmPassword);
        Task<TokenResponse> ChangePassword(string oldPassword, string newPassword, string confirmPassword);
        Task<TokenResponse> VerifyRegisterOtp(UserOtpType type, string email, string phone, string otp, string otpToken);
        Task<RefreshTokenResponse> VerifyRefreshToken(string refreshToken);
        Task<bool> DeleteAccount(DeleteUserReq request);
    }
}

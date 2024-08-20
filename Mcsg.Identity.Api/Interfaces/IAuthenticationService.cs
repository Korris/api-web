namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
using Common.Core.Enums;
using Requests;
using Response;

public interface IAuthenticationService
{
    Task<VerifyUserResponse> RegisterUser(RegisterUserReq request);
    Task<VerifyUserResponse> ResendOtp(ResendOtpReq request);
    Task<TokenDto> LoginUser(LoginUserReq request);
    Task<TokenDto> LoginSocial(LoginSocialReq request);
    Task<bool> LogOut();
    Task<VerifyUserResponse> ForgotPassword(string? email, string? phone);
    Task<bool> ResetPassword(ResetPasswordReq request);
    Task<TokenDto> SetUserPassword(string password, string confirmPassword);
    Task<bool> CreateNewUserPassword(string? email, string? phone, string otp, string otpToken, string password, string confirmPassword);
    Task<TokenDto> ChangePassword(string oldPassword, string newPassword, string confirmPassword);
    Task<TokenDto> VerifyRegisterOtp(UserOtpType type, string? email, string? phone, string otp, string otpToken);
    Task<RefreshTokenResponse> VerifyRefreshToken(string refreshToken);
    Task<bool> DeleteUser(AuthenticationDeleteUserR request);
    Task CheckRegisterUser(RegisterUserReq request);
}

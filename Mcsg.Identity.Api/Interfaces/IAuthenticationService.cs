namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
using Requests;
using Response;

public interface IAuthenticationService
{
    Task CheckRegisterUser(AuthenticationRegisterUserR request);
    Task<VerifyUserResponse> RegisterUser(AuthenticationRegisterUserR request);
    Task<TokenDto> LoginUser(AuthenticationLoginUserR request);
    Task<TokenDto> LoginSocial(AuthenticationLoginSocialR request);
    Task<bool> LogOut();
    Task<VerifyUserResponse> ResendOtp(AuthenticationResendOtpR request);
    Task<TokenDto> ChangePassword(AuthenticationChangePasswordR request);
    Task<TokenDto> SetUserPassword(string password, string confirmPassword);
    Task<VerifyUserResponse> ForgotPassword(AuthenticationForgotPasswordR request);
    Task<bool> ResetPassword(AuthenticationResetPasswordR request);
    Task<bool> CreateNewUserPassword(AuthenticationSetPasswordR request);
    Task<RefreshTokenResponse> VerifyRefreshToken(AuthenticationRefreshTokenR request);
    Task<bool> DeleteUser(AuthenticationDeleteUserR request);
}

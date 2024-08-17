using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers;

using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="authenticationService"></param>
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("check-register-info")]
    public async Task<IActionResult> CheckRegisterUser(RegisterUserReq request)
    {
        request.Analyze(HttpContext);
        await _authenticationService.CheckRegisterUser(request);
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.RegisterUser(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.LoginUser(request);
        return Ok(result);
    }

    [HttpPost("social-login")]
    public async Task<IActionResult> LoginSocial(LoginSocialReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.LoginSocial(request);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authenticationService.LogOut();
        return Ok(result);
    }

    /// <summary>
    /// ResendOtp with Auth no need token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(ResendOtpReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ResendOtp(request.Type, request.OtpToken);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.VerifyRefreshToken(request.RefreshToken);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ForgotPassword(request.Email, request.Phone);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ResetPassword(request);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ChangePassword(request.OldPassword, request.NewPassword, request.ConfirmPassword);
        return Ok(result);
    }

    [HttpPost("create-new-user-password")]
    public async Task<IActionResult> CreateNewUserPassword(CreateNewUserPasswordReq request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.CreateNewUserPassword(request.Email, request.Phone, request.Otp, request.OtpToken, request.Password, request.ConfirmPassword);
        return Ok(result);
    }

    [HttpDelete("delete-account"), Authorize]
    public async Task<IActionResult> DeleteUser(AuthenticationDeleteUserR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.DeleteUser(request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IAuthenticationService _authenticationService;

    #endregion
}

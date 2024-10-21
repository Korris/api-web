using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers;

using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Setting;

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
    public async Task<IActionResult> CheckRegisterUser(AuthenticationRegisterUserR request)
    {
        request.Analyze(HttpContext);
        await _authenticationService.CheckRegisterUser(request);
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthenticationRegisterUserR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.RegisterUser(request);
        return Ok(result);
    }

    [HttpPost("create-account"), Authorize(Roles = McsgRole.Admin)]
    public async Task<IActionResult> CreateAccount([FromBody] AuthenticationRegisterUserR request)
    {
        request.SetForAdmin(true);
        request.Analyze(HttpContext);
        var result = await _authenticationService.RegisterUser(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthenticationLoginUserR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.LoginUser(request);
        return Ok(result);
    }

    [HttpPost("social-login")]
    public async Task<IActionResult> LoginSocial(AuthenticationLoginSocialR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.LoginSocial(request);
        return Ok(result);
    }

    [HttpPost("logout"), Authorize]
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
    public async Task<IActionResult> ResendOtp(AuthenticationResendOtpR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ResendOtp(request);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(AuthenticationRefreshTokenR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.VerifyRefreshToken(request);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(AuthenticationForgotPasswordR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ForgotPassword(request);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(AuthenticationResetPasswordR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ResetPassword(request);
        return Ok(result);
    }

    [HttpPut("change-password"), Authorize]
    public async Task<IActionResult> ChangePassword(AuthenticationChangePasswordR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.ChangePassword(request);
        return Ok(result);
    }

    [HttpPost("create-new-user-password")]
    public async Task<IActionResult> CreateNewUserPassword(AuthenticationSetPasswordR request)
    {
        request.Analyze(HttpContext);
        var result = await _authenticationService.CreateNewUserPassword(request);
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

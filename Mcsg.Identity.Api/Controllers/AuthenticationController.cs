using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers
{
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserReq request)
        {
            var result = await _authenticationService.RegisterUser(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserReq request)
        {
            var result = await _authenticationService.LoginUser(request);
            return Ok(result);
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> LoginSocial(LoginSocialReq request)
        {
            var result = await _authenticationService.SocialLogin(request.SocialType, request.SocialToken);
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
        /// <param name="resendOtpReq"></param>
        /// <returns></returns>
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(ResendOtpReq resendOtpReq)
        {
            var result = await _authenticationService.ResendOtp(resendOtpReq.Type, resendOtpReq.OtpToken);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenReq request)
        {
            var result = await _authenticationService.VerifyRefreshToken(request.RefreshToken);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordReq request)
        {
            var result = await _authenticationService.ForgotPassword(request.Email, request.Phone);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordReq request)
        {
            var result = await _authenticationService.ResetPassword(request);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordReq request)
        {
            var result = await _authenticationService.ChangePassword(request.OldPassword, request.NewPassword, request.ConfirmPassword);
            return Ok(result);
        }

        [HttpPost("create-new-user-password")]
        public async Task<IActionResult> CreateNewUserPassword(CreateNewUserPasswordReq request)
        {
            var result = await _authenticationService.CreateNewUserPassword(request.Email, request.Phone, request.Otp, request.OtpToken, request.Password, request.ConfirmPassword);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount(DeleteUserReq request)
        {
            var result = await _authenticationService.DeleteAccount(request);
            return Ok(result);
        }

        #endregion

        #region -- Fields --

        private readonly IAuthenticationService _authenticationService;

        #endregion
    }
}

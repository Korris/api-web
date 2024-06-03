using Mcsg.Identity.Api.DTOs.Request;
using Mcsg.Identity.Api.Services.Interface;
using Mcsg.Identity.Api.Services.Interfaces;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers
{
    /// <summary>
    /// Use for user logged in
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        public OtpController(IOtpService otpService,
            ICurrentUserService currentUserService,
            IUserService userService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _otpService = otpService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOtp(CreateOtpReq request)
        {
            await _otpService.ClearAllUserOtpAsync(_currentUserService.Session.UserId, request.Type);
            var to = request.Type == UserOtpType.ConfirmEmail ? request.Email : request.Phone;
            var result = await _otpService.CreateAsync(_currentUserService.Session.UserId, to, request.Type);
            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp(VerifyRegisterOtpReq request)
        {
            var result = await _otpService.VerifyAsync(request.OtpToken, request.Otp, request.Type);
            if (request.Type == UserOtpType.ConfirmPhone)
            {
                result = true; // by pass for phone, update later
            }
            if (result)
            {
                if (request.Type == UserOtpType.ConfirmEmail)
                {
                    await _userService.ConfirmEmailAsync(request.Email);
                }
                else if (request.Type == UserOtpType.ConfirmPhone)
                {
                    await _userService.ConfirmPhoneNumberAsync(request.Phone);
                }

                await _otpService.ClearAllUserOtpAsync(_currentUserService.Session.UserId, request.Type);
            }
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers
{
    using Interfaces;
    using Lib.Common.Web.Security;
    using Lib.Data.Enums;
    using Requests;

    /// <summary>
    /// Use for user logged in
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OtpController : ControllerBase
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="otpService"></param>
        /// <param name="currentUserService"></param>
        /// <param name="userService"></param>
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

        #endregion

        #region -- Fields --

        private readonly IOtpService _otpService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        #endregion
    }
}

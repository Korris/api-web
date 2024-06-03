using Mcsg.Admin.Api.DTOs.Settings;
using Mcsg.Admin.Api.Services.Interface;
using Mcsg.Lib.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Admin.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = RoleNames.SysAdmin)]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;
        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        [HttpGet("global-setting")]
        public async Task<IActionResult> GetGlobalSetting()
        {
            var result = await _systemSettingService.GetGlobalSettingAsync();
            return Ok(result);
        }

        [HttpPut("global-setting/{id}")]
        public async Task<IActionResult> UpdateGlobalSetting(Guid id, [FromForm] GlobalSettingRequest request)
        {
            var result = await _systemSettingService.UpdateGlobalSettingAsync(id, request);
            return Ok(result);
        }

        [HttpGet("email-setting")]
        public async Task<IActionResult> GetEmailSetting()
        {
            var result = await _systemSettingService.GetEmailSettingAsync();
            return Ok(result);
        }

        [HttpPut("email-setting/{id}")]
        public async Task<IActionResult> UpdateEmailSetting(Guid id, [FromBody] EmailSettingRequest request)
        {
            var result = await _systemSettingService.UpdateEmailSettingAsync(id, request);
            return Ok(result);
        }
    }
}

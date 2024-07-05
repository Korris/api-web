using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Admin.Api.Controllers
{
    using Lib.Common.Constants;
    using Requests;
    using Services.Interface;

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.SysAdmin}")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("list-user")]
        public async Task<IActionResult> GetListUser([FromQuery] UserListRequest request)
        {
            var result = await _userService.GetListUserAsync(request);
            return Ok(result);
        }

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> ChangeUserStatus(Guid id, [FromBody] UserStatusReq request)
        {
            var result = await _userService.ChangeStatusAsync(id, request);
            return Ok(result);
        }

        [HttpPost("create-admin")]
        [Authorize(Roles = RoleNames.SysAdmin)]
        public async Task<IActionResult> CreateAdminUser([FromBody] CreateAdminReq request)
        {
            var result = await _userService.CreateAdminAsync(request);
            return Ok(result);
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = RoleNames.SysAdmin)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
}

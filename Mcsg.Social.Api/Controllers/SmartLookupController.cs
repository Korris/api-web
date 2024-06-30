using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using Interfaces;
    using Models;
    using Requests;

    [ApiController]
    [Route("[controller]")]
    public class SmartLookupController : ControllerBase
    {
        private readonly ISmartLookupService _smartLookupService;
        private readonly IUserService _userService;

        public SmartLookupController(ISmartLookupService smartLookupService, IUserService userService)
        {
            _smartLookupService = smartLookupService;
            _userService = userService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] object keyword)
        {
            var result = await _smartLookupService.SearchAsync(keyword?.ToString());
            return Ok(result);
        }


        [HttpGet("recent-search")]
        [Authorize]
        public async Task<IActionResult> GetRecentSearch()
        {
            var result = await _smartLookupService.GetRecentListAsync();
            return Ok(result);
        }

        [HttpDelete("remove-recent-search/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveRecentSearch(Guid id)
        {
            var result = await _smartLookupService.DeleteRecentSearchAsync(id);
            return Ok(result);
        }

        [HttpPost("add-recent-search")]
        [Authorize]
        public async Task<IActionResult> AddRecentSearch([FromBody] AddRecentSearchRequest req)
        {
            var result = await _smartLookupService.AddRecentSearchAsync(req);
            return Ok(result);
        }

        [HttpGet("user-list")]
        public async Task<IActionResult> SearchUserByKeyWord([FromQuery] SearchUserReq input)
        {
            var result = await _userService.SearchUserbyKeyword(input);
            return Ok(result);
        }
    }
}

using Mcsg.Api.Models;
using Mcsg.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmartLookupController : ControllerBase
    {
        private readonly ISmartLookupService _smartLookupService;
        public SmartLookupController(ISmartLookupService smartLookupService)
        {
            _smartLookupService = smartLookupService;
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
    }
}

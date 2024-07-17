using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Comic.Api.Controllers;

using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class SmartLookupController : ControllerBase
{
    #region -- Methods --

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
        var r = new BaseR(HttpContext);
        var result = await _smartLookupService.GetRecentListAsync(r.UserId!.Value);
        return Ok(result);
    }

    [HttpDelete("remove-recent-search/{id}")]
    [Authorize]
    public async Task<IActionResult> RemoveRecentSearch(Guid id)
    {
        var result = await _smartLookupService.DeleteRecentSearchAsync(id);
        return Ok(result);
    }

    [HttpPost("add-recent-search"), Authorize]
    public async Task<IActionResult> AddRecentSearch([FromBody] SmartLookupAddRecentSearchR req)
    {
        var r = new BaseR(HttpContext);
        var result = await _smartLookupService.AddRecentSearchAsync(req, r.UserId!.Value);
        return Ok(result);
    }

    [HttpGet("user-list")]
    public async Task<IActionResult> SearchUserByKeyWord([FromQuery] SmartLookupSearchUserR input)
    {
        var result = await _userService.SearchUserbyKeyword(input);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly ISmartLookupService _smartLookupService;

    private readonly IUserService _userService;

    #endregion
}

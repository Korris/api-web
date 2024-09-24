using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Interfaces;
using Requests;

[ApiController]
[Route("[controller]"), Authorize]
public class UserPurchaseController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="userPurchaseService"></param>
    public UserPurchaseController(IUserPurchaseService userPurchaseService)
    {
        _userPurchaseService = userPurchaseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPurchaseList([FromQuery] UserPurchasePaginatedR request)
    {
        var result = await _userPurchaseService.GetUserPurchaseTransactionsAsync(request);
        return Ok(result);
    }

    [HttpGet("current-package")]
    public async Task<IActionResult> GetUserPackageList()
    {
        var result = await _userPurchaseService.GetUserPremiumPackageAsync();
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IUserPurchaseService _userPurchaseService;

    #endregion
}

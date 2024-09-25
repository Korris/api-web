using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]"), Authorize]
public class PremiumController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="premiumService"></param>
    public PremiumController(IPremiumService premiumService)
    {
        _premiumService = premiumService;
    }

    [HttpGet("packages")]
    public async Task<IActionResult> GetUserWalletInfo()
    {
        var result = await _premiumService.GetPremiumPackage();
        return Ok(result);
    }

    [HttpGet("select-package/{packageNo}")]
    public async Task<IActionResult> GetSelectPackage(int packageNo)
    {
        var req = new BaseR(HttpContext);
        var result = await _premiumService.SelectPremiumPackage(req.UserId ?? Guid.Empty, packageNo);
        return Ok(result);
    }

    [HttpPost("buy-chapter")]
    public async Task<IActionResult> BuyChapter(PremiumBuyChapterR req)
    {
        req.Analyze(HttpContext);
        var result = await _premiumService.BuyChapter(req);
        return Ok(result);
    }

    [HttpPost("buy-serie")]
    public async Task<IActionResult> BuySeries(PremiumBuySerieR req)
    {
        req.Analyze(HttpContext);
        var result = await _premiumService.BuySerieAsync(req);
        return Ok(result);
    }

    [HttpPost("buy-premium")]
    public async Task<IActionResult> BuyPremium(PremiumBuyPremiumR req)
    {
        req.Analyze(HttpContext);
        var result = await _premiumService.BuyPremium(req);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IPremiumService _premiumService;

    #endregion
}

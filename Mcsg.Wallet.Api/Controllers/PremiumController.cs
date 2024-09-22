using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Common.Domain.Entities;
using Domain;
using Interfaces;
using Lib.Data.Repositories;
using Requests;

[ApiController]
[Route("[controller]")]
//[Authorize]
public class PremiumController : ControllerBase
{
    private readonly IPremiumService _premiumService;
    readonly WalletContext _walletDbContext;
    IRepository<User> _repository;

    public PremiumController(IPremiumService premiumService,
        WalletContext walletDbContext,
        IOtpService otpService,
        IRepository<User> repository)
    {
        _repository = repository;
        _walletDbContext = walletDbContext;
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
        var result = await _premiumService.SelectPremiumPackage(packageNo);
        return Ok(result);
    }

    [HttpPost("buy-chapter")]
    public async Task<IActionResult> BuyChapter(PremiumBuyChapterR req)
    {
        var result = await _premiumService.BuyChapter(req);
        return Ok(result);
    }

    [HttpPost("buy-serie")]
    public async Task<IActionResult> BuySeries(PremiumBuySerieR req)
    {
        var result = await _premiumService.BuySerieAsync(req);
        return Ok(result);
    }

    [HttpPost("buy-premium")]
    public async Task<IActionResult> BuyPremium(PremiumBuyPremiumR req)
    {
        var result = await _premiumService.BuyPremium(req);
        return Ok(result);
    }
}

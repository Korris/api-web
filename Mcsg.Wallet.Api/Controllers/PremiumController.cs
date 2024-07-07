using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Interfaces;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Wallet;
using Models;

[ApiController]
[Route("[controller]")]
//[Authorize]
public class PremiumController : ControllerBase
{
    private readonly IPremiumService _premiumService;
    private readonly IOtpService _otpService;
    readonly WalletDbContext _walletDbContext;
    IRepository<User> _repository;
    public PremiumController(IPremiumService premiumService,
        WalletDbContext walletDbContext,
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
    public async Task<IActionResult> BuyChapter(BuyChapterReq req)
    {
        var result = await _premiumService.BuyChapter(req);
        return Ok(result);
    }
    [HttpPost("buy-serie")]
    public async Task<IActionResult> BuySeries(BuySerieReq req)
    {
        var result = await _premiumService.BuySerieAsync(req);
        return Ok(result);
    }
    [HttpPost("buy-premium")]
    public async Task<IActionResult> BuyPremium(BuyPremiumReq req)
    {
        var result = await _premiumService.BuyPremium(req);
        return Ok(result);
    }
}

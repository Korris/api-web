using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Lib.Common.Models;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Wallet;
using Services;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserPurchaseController : ControllerBase
{//https://vietqr.io/paymentRequests/#operation/paymentLink
    //https://vietqr.io/danh-sach-api/api-danh-sach-ma-ngan-hang
    private readonly IUserPurchaseService _userPurchaseService;
    private readonly IOtpService _otpService;
    readonly WalletDbContext _walletDbContext;
    IRepository<User> _repository;
    public UserPurchaseController(IUserPurchaseService userPurchaseService,
        WalletDbContext walletDbContext,
        IOtpService otpService,
        IRepository<User> repository)
    {
        _repository = repository;
        _walletDbContext = walletDbContext;
        _userPurchaseService = userPurchaseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPurchaseList([FromQuery] PaginatedRequest request)
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

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Interfaces;

[ApiController]
[Route("[controller]"), Authorize]
public class WalletController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="bankService"></param>
    public WalletController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpGet("bank-list")]
    public async Task<IActionResult> GetBankList()
    {
        var result = await _bankService.GetBanks();
        return Ok(result);
    }

    [HttpPut("sync-bank-list")]
    public async Task<IActionResult> SyncBankList()
    {
        var result = await _bankService.SyncBanks();
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IBankService _bankService;

    #endregion
}

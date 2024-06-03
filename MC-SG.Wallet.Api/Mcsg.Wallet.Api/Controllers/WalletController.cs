using Mcsg.Wallet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IBankService _bankService;

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
        [Authorize]
        [HttpPut("sync-bank-list")]
        public async Task<IActionResult> SyncBankList()
        {
            var result = await _bankService.SyncBanks();
            return Ok(result);
        }
    }
}

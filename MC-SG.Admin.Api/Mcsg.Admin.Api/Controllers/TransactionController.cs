using Mcsg.Admin.Api.DTOs.Transactions;
using Mcsg.Admin.Api.Services.Interface;
using Mcsg.Lib.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Admin.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.SysAdmin}")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        // [Authorize(Roles = RoleNames.SysAdmin)]
        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] UserWalletTransactionReq request)
        {
            var result = await _transactionService.GetListAsync(request);
            return Ok(result);
        }
        [HttpPut("approve")]
        public async Task<IActionResult> Approve(ApproveReq req)
        {
            var result = await _transactionService.ApproveTransaction(req);
            return Ok(result);
        }
        [HttpPut("reject")]
        public async Task<IActionResult> Reject(RejectReq req)
        {
            var result = await _transactionService.RejectTransaction(req);
            return Ok(result);
        }
        [HttpPut("approveRef")]
        public async Task<IActionResult> ApproveRef(ApproveRefReq req)
        {
            var result = await _transactionService.ApproveRefTransaction(req);
            return Ok(result);
        }
        [HttpPut("rejectRef")]
        public async Task<IActionResult> RejectRef(RejectRefReq req)
        {
            var result = await _transactionService.RejectRefTransaction(req);
            return Ok(result);
        }
    }
}

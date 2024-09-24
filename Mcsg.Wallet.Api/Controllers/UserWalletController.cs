using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Wallet.Api.Controllers;

using Domain.Enums;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]"), Authorize]
public class UserWalletController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="userWalletService"></param>
    /// <param name="zaloPayService"></param>
    public UserWalletController(IUserWalletService userWalletService, IZaloPayService zaloPayService)
    {
        _userWalletService = userWalletService;
        _zaloPayService = zaloPayService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWalletInfo()
    {
        var result = await _userWalletService.GetUserWalletAsync();
        return Ok(result);
    }

    [HttpGet("info/{address}")]
    public async Task<IActionResult> GetUserBasicWallet(string address)
    {
        var result = await _userWalletService.GetUserWalletByAddressAsync(address);
        return Ok(result);
    }

    [HttpGet("payment-method")]
    public async Task<IActionResult> GetUserPaymentMethod()
    {
        var result = await _userWalletService.GetUserPaymentMethods();
        return Ok(result);
    }

    [HttpPost("payment-method/add")]
    public async Task<IActionResult> AddUserPaymentMethod([FromBody] UserWalletAddPaymentMethodR addUserPaymentMethodReq)
    {
        var result = await _userWalletService.AddUserPaymentMethod(addUserPaymentMethodReq);
        return Ok(result);
    }

    [HttpPut("payment-method/{userPaymentMethodId}")]
    public async Task<IActionResult> UpdateUserPaymentMethod(Guid userPaymentMethodId, [FromBody] UserWalletUpdatePaymentMethodR updateUserPaymentMethodReq)
    {
        var result = await _userWalletService.UpdateUserPaymentMethod(userPaymentMethodId, updateUserPaymentMethodReq);
        return Ok(result);

    }

    [HttpDelete("payment-method/remove/{paymentMethodId}")]
    public async Task<IActionResult> RemoveUserPaymentMethod(Guid paymentMethodId)
    {
        var result = await _userWalletService.RemoveUserPaymentMethod(paymentMethodId);
        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetUserWalletTransactions(int page = 1, int pageSize = 10)
    {
        var result = await _userWalletService.GetUserWalletTransactionsAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("transaction/{referenceNumber}")]
    public async Task<IActionResult> UserWalletTransactionDetail(string referenceNumber)
    {
        var result = await _userWalletService.GetUserWalletTransactionByRefNumberAsync(referenceNumber);
        return Ok(result);
    }

    [HttpPost("donate")]
    public async Task<IActionResult> Donate(UserWalletDonateR req)
    {
        var result = await _userWalletService.DonateAsync(req);
        return Ok(result);
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(UserWalletTransferR req)
    {
        var result = await _userWalletService.TransferAsync(req);
        return Ok(result);
    }

    [HttpGet("deposit-prepare")]
    public async Task<IActionResult> PrepareDeposit()
    {
        var result = await _userWalletService.DepositPrepareAsync();
        return Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(UserWalletDepositR req)
    {
        var result = await _userWalletService.DepositAsync(req);
        return Ok(result);
    }

    [HttpPost("deposit-cancel")]
    public async Task<IActionResult> DepositCancel(UserWalletDepositCancelR req)
    {
        var result = await _userWalletService.DepositCancelAsync(req);
        return Ok(result);
    }

    [HttpGet("withdraw-prepare")]
    public async Task<IActionResult> PrepareWithdraw()
    {
        var result = await _userWalletService.WithdrawPrepareAsync();
        return Ok(result);
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(UserWalletWithdrawR req)
    {
        var result = await _userWalletService.WithdrawAsync(req);
        return Ok(result);
    }

    [HttpPost("transaction/resendOtp")]
    public async Task<IActionResult> ResentTransactionOtp(Guid transactionId, TransactionOtpType otpType)
    {
        var result = await _userWalletService.ResentTransactionOtpAsync(transactionId, otpType);
        return Ok(result);
    }

    [HttpPost("transaction/verifyOtp")]
    public async Task<IActionResult> VerifyTransactionOtp(UserWalletVerifyTransactionOtpR req)
    {
        var result = await _userWalletService.VerifyTransactionOtpAsync(req);
        return Ok(result);
    }

    [HttpPost("zalopay-callback")]
    public async Task<IActionResult> CallBackZaloPayAsync(UserWalletZaloPayCallBackR req)
    {
        var result = await _userWalletService.CallBackZaloPayAsync(req);
        return Ok(result);
    }

    [HttpPost("callback-zalopay")]
    public IActionResult CallBackZaloPay(dynamic cbdata)
    {
        var result = _userWalletService.CallBackZaloPay(cbdata);
        return Ok(result);
    }

    [HttpPost("zalopay-query")]
    public async Task<IActionResult> QueryZaloPay(string transactionId)
    {
        if (!string.IsNullOrEmpty(transactionId))
        {
            var transId = Guid.Parse(transactionId);
            var result = await _zaloPayService.QueryOrderAsync(transId);
            return Ok(result);
        }
        return BadRequest();
    }

    [HttpGet("wallet-info/{username}")]
    public async Task<IActionResult> GetWalletAddressByUsername(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            var result = await _userWalletService.GetUserWalletAddressByUsername(username);
            return Ok(result);
        }
        return BadRequest();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// https://vietqr.io/paymentRequests/#operation/paymentLink
    /// https://vietqr.io/danh-sach-api/api-danh-sach-ma-ngan-hang
    /// </summary>
    private readonly IUserWalletService _userWalletService;

    private readonly IZaloPayService _zaloPayService;

    #endregion
}

using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mcsg.Wallet.Job.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Extensions;
using Common.Models;
using Common.Models.RealTime;
using Domain.Enums;
using Domain.Interfaces;
using Interfaces;
using Job.Constants;

public class PaymentService : BaseS, IPaymentService
{
    #region -- Methods --

    public PaymentService(IWalletContext context, ILogger<PaymentService> logger, ISetting setting) : base(context)
    {
        _logger = logger;
        _setting = setting;
    }

    public async Task ZPQueryOrderAsync(PaymentTransData data)
    {
        var transaction = await _context.WalletTransactions
                                    .Include(x => x.SourceUserWallet)
                                    .FirstOrDefaultAsync(x => x.Id == data.TransactionId);
        if (transaction != null)
        {
            if (transaction.Status == TransactionStatus.Pending)
            {
                var appTransId = transaction.ExternalId;
                _logger.LogInformation($"PaymentService-ZPQueryOrderAsync-TransId: {transaction.Id} - appTransId: {appTransId}");
                if (!string.IsNullOrEmpty(appTransId))
                {
                    var appId = _setting.ZaloPay.AppId;
                    var key1 = _setting.ZaloPay.Key1;
                    var url = _setting.ZaloPay.Url;

                    var param = new Dictionary<string, string>
                    {
                        { "app_id", appId },
                        { "app_trans_id", appTransId }
                    };
                    var macData = appId + "|" + appTransId + "|" + key1;

                    param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, macData));

                    var responseData = new QueryZalopayPayResponse();
                    responseData.ReturnCode = (int)ZaloPayReturnCode.PROCESSING;

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    while (responseData.ReturnCode == (int)ZaloPayReturnCode.PROCESSING)
                    {
                        if (stopwatch.Elapsed > TimeSpan.FromMinutes(17))
                        {
                            break;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        using var client = new HttpClient();
                        var content = new FormUrlEncodedContent(param);
                        var response = client.PostAsync(url.QueryZpUrl(), content).Result;

                        _logger.LogInformation($"PaymentService-ZPQueryOrderAsync-TransId: {transaction.Id} - api.response: {JsonConvert.SerializeObject(response)}");
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            responseData = JsonConvert.DeserializeObject<QueryZalopayPayResponse>(responseContent);
                        }
                    }
                    stopwatch.Stop();

                    if (responseData.ReturnCode != (int)ZaloPayReturnCode.PROCESSING)
                    {
                        _logger.LogInformation($"PaymentService-ZPQueryOrderAsync-TransId: {transaction.Id} - responseCode: {responseData.ReturnCode}");
                        transaction.Status = responseData.ReturnCode == (int)ZaloPayReturnCode.SUCCESS ? TransactionStatus.Success : TransactionStatus.Failed;
                        if (responseData.ReturnCode == (int)ZaloPayReturnCode.SUCCESS)
                        {
                            transaction.SourceUserWallet.Point += transaction.Amount;
                        }
                        var result = await _context.SaveChangesAsync(default);

                        if (result > 0)
                        {
                            var realTimeReq = new RealTimeTransactionUpdateReq()
                            {
                                PaymentType = PaymentMethodType.ZaloPay,
                                ReferenceNumber = transaction.ReferenceNumber,
                                TransactionId = data.TransactionId.ToString(),
                                TransactionStatus = transaction.Status,
                                UserId = data.UserId,
                                WalletAddress = transaction.SourceUserWallet.Address,
                                Point = transaction.SourceUserWallet.Point,
                                RewardPoint = transaction.SourceUserWallet.RewardPoint,
                                TotalPoint = transaction.SourceUserWallet.Point + transaction.SourceUserWallet.RewardPoint
                            };
                            await AddTransactionUpdateNotificationAsync(realTimeReq);
                        }
                    }
                }
            }
        }
    }

    private async Task<bool> AddTransactionUpdateNotificationAsync(RealTimeTransactionUpdateReq req)
    {
        var baseUrl = Environment.GetEnvironmentVariable(FunctionConstant.RealTimeServiceUrl);
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/transaction-update");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    private readonly ILogger<PaymentService> _logger;

    #endregion
}

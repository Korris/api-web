using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mcsg.Function.Job.Services
{
    using Interfaces;
    using Job.Constants;
    using Lib.Common.Enums;
    using Lib.Common.Extensions;
    using Lib.Common.Helpers;
    using Lib.Common.Models;
    using Lib.Common.Models.RealTime;
    using Lib.Data.Repositories.Interface;
    using Lib.Data.Wallet;
    using Lib.Data.Wallet.Enums;

    public class PaymentService : IPaymentService
    {
        public PaymentService(IUnitOfWork unitOfWork, WalletDbContext walletDbContext, ILogger<PaymentService> logger, ISetting setting)
        {
            _unitOfWork = unitOfWork;
            _walletDbContext = walletDbContext;
            _logger = logger;
            _setting = setting;
        }

        public async Task ZPQueryOrderAsync(PaymentTransData data)
        {
            var transaction = await _walletDbContext.WalletTransactions
                                        .Include(x => x.SourceUserWallet)
                                        .FirstOrDefaultAsync(x => x.Id == data.TransactionId);
            if (transaction != null)
            {
                if (transaction.Status == TransactionStatus.PENDING)
                {
                    var appTransId = transaction.ExternalId;
                    _logger.LogInformation($"PaymentService-ZPQueryOrderAsync-TransId: {transaction.Id} - appTransId: {appTransId}");
                    if (!string.IsNullOrEmpty(appTransId))
                    {
                        var appId = _setting.ZaloPay.AppId;
                        var key1 = _setting.ZaloPay.Key1;
                        var url = _setting.ZaloPay.Url;

                        var param = new Dictionary<string, string>();
                        param.Add("app_id", appId);
                        param.Add("app_trans_id", appTransId);
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
                            transaction.Status = responseData.ReturnCode == (int)ZaloPayReturnCode.SUCCESS ? TransactionStatus.SUCCESS : TransactionStatus.FAILED;
                            if (responseData.ReturnCode == (int)ZaloPayReturnCode.SUCCESS)
                            {
                                transaction.SourceUserWallet.Point += transaction.Amount;
                            }
                            var result = await _walletDbContext.SaveChangesAsync();

                            if (result > 0)
                            {
                                var realTimeReq = new RealTimeTransactionUpdateReq()
                                {
                                    PaymentType = PaymentMethodType.ZALO_PAY,
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

            var response = await HttpHelper.MakePostRequest(url, req);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region -- Fields --

        private readonly WalletDbContext _walletDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}

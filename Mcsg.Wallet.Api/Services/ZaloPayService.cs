using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Constants;
using Domain;
using Interfaces;
using Lib.Common.Extensions;
using Lib.Common.Models;
using Lib.Common.Models.RealTime;
using Lib.Common.Web.RealTime.Services;
using Lib.Common.Web.Security;
using Models._3rdClass.ZaloPay.Request;
using Models._3rdClass.ZaloPay.Response;

public class ZaloPayService : IZaloPayService
{
    private readonly ZaloPaySetting _zaloPaySetting;
    private readonly ISignalRService _signalRService;
    private readonly WalletContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ZaloPayService(WalletContext walletDbContext,
        ICurrentUserService currentUserService,
        ISignalRService signalRService,
        IOptions<ZaloPaySetting> zaloPaySettingOptions,
        ISetting setting)
    {
        _dbContext = walletDbContext;
        _currentUserService = currentUserService;
        _signalRService = signalRService;
        _zaloPaySetting = zaloPaySettingOptions.Value;
        _setting = setting;
    }

    public async Task CompleteTransactionAsync(Guid transactionId, Guid userId, TransactionStatus status)
    {
        try
        {
            var transaction = await _dbContext.WalletTransactions
                                        .Include(x => x.SourceUserWallet)
                                        .FirstOrDefaultAsync(x => x.Id == transactionId);
            if (transaction != null)
            {
                // Update to db.
                transaction.Status = status;
                if (status == TransactionStatus.Success)
                {
                    transaction.SourceUserWallet.Point += transaction.Amount;
                }

                await _dbContext.SaveChangesAsync();

                // Send signalR to user.
                var realTimeReq = new RealTimeTransactionUpdateReq()
                {
                    PaymentType = PaymentMethodType.ZaloPay,
                    ReferenceNumber = transaction.ReferenceNumber,
                    TransactionId = transactionId.ToString(),
                    TransactionStatus = status,
                    UserId = userId,
                    WalletAddress = transaction.SourceUserWallet.Address,
                    Point = transaction.SourceUserWallet.Point,
                    RewardPoint = transaction.SourceUserWallet.RewardPoint,
                    TotalPoint = transaction.SourceUserWallet.Point + transaction.SourceUserWallet.RewardPoint
                };
                await AddTransactionUpdateNotificationAsync(realTimeReq);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CreateOrderResponse> CreateOrderAsync(Guid transactionId, string content, float amount, Guid? userId, string redirectUrl)
    {
        try
        {
            var embed_data = new ZaloPayEmbedData()
            {
                RedirectUrl = redirectUrl,
                TransactionId = transactionId.ToString(),
                UserId = userId.ToString()
            };
            var items = new List<ZaloPayItemData>();
            Random rnd = new Random();
            var idRandom = rnd.Next(1000000000);
            var appTransId = DateTime.Now.ToString("yyMMdd") + "_" + idRandom;

            var zalopayPayRequest = new CreateZalopayPayRequest(_zaloPaySetting.AppId, _zaloPaySetting.AppUser,
            DateTime.Now.GetTimeStamp().ToString(), (long)amount!,
            appTransId,
                            SystemSettings.ZALO_PAY_BANK_CODE,
                            content ?? string.Empty,
                            _zaloPaySetting.CallBackUrl,
                            embed_data, items);
            zalopayPayRequest.MakeSignature(_zaloPaySetting.Key1);
            var createOrderRes = zalopayPayRequest.GetLink(_zaloPaySetting.Url.CreateZpUrl());
            if (createOrderRes != null)
            {
                var orderRes = new CreateOrderResponse()
                {
                    AppTransId = appTransId,
                    ReturnCode = createOrderRes.ReturnCode,
                    ReturnMessage = createOrderRes.ReturnMessage,
                    SubReturnCode = createOrderRes.SubReturnCode,
                    SubReturnMessage = createOrderRes.SubReturnMessage,
                    ZpTransToken = createOrderRes.ZpTransToken,
                    OrderToken = createOrderRes.OrderToken,
                    OrderUrl = createOrderRes.OrderUrl,
                    QrCode = createOrderRes.QrCode
                };
                return orderRes;
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<QueryZalopayPayResponse> QueryOrderAsync(Guid transactionId)
    {
        var transaction = await _dbContext.WalletTransactions.FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction != null)
        {
            if (transaction.Status == TransactionStatus.Pending)
            {
                var appTransId = transaction.ExternalId;
                if (!string.IsNullOrEmpty(appTransId))
                {
                    var param = new Dictionary<string, string>();
                    param.Add("app_id", _zaloPaySetting.AppId.ToString());
                    param.Add("app_trans_id", appTransId);
                    var data = _zaloPaySetting.AppId.ToString() + "|" + appTransId + "|" + _zaloPaySetting.Key1;

                    param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _zaloPaySetting.Key1, data));

                    using var client = new HttpClient();
                    var content = new FormUrlEncodedContent(param);
                    var response = client.PostAsync(_zaloPaySetting.Url.QueryZpUrl(), content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        var responseData = JsonConvert.DeserializeObject<QueryZalopayPayResponse>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        return null;
    }

    private async Task<bool> AddTransactionUpdateNotificationAsync(RealTimeTransactionUpdateReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        if (string.IsNullOrEmpty(baseUrl))
        {
            return false;
        }
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

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}

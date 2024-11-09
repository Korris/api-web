using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Extensions;
using Common.Models;
using Common.Models.RealTime;
using Domain.Interfaces;
using Interfaces;
using Models._3rdClass.ZaloPay.Request;
using Models._3rdClass.ZaloPay.Response;

public class ZaloPayService : BaseSettingS, IZaloPayService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    public ZaloPayService(IWalletContext context, ISetting setting) : base(context, setting) { }

    public async Task CompleteTransactionAsync(Guid transactionId, Guid userId, TransactionStatus status)
    {
        try
        {
            var transaction = await _context.WalletTransactions
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

                await _context.SaveChangesAsync(default);

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

            var zp = _setting.ZaloPay;
            var now = DateTime.Now.GetTimeStamp().ToString();
            var zalopayPayRequest = new CreateZalopayPayRequest(zp.AppId, zp.AppUser, now, (long)amount!, appTransId, zp.ConfigName, content, zp.CallBackUrl, embed_data, items);
            zalopayPayRequest.MakeSignature(zp.Key1);
            var createOrderRes = zalopayPayRequest.GetLink(zp.Url.CreateZpUrl());
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
        var transaction = await _context.WalletTransactions.FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction != null)
        {
            if (transaction.Status == TransactionStatus.Pending)
            {
                var appTransId = transaction.ExternalId;
                if (!string.IsNullOrEmpty(appTransId))
                {
                    var param = new Dictionary<string, string>
                    {
                        { "app_id", _setting.ZaloPay.AppId.ToString() },
                        { "app_trans_id", appTransId }
                    };
                    var data = _setting.ZaloPay.AppId.ToString() + "|" + appTransId + "|" + _setting.ZaloPay.Key1;

                    param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _setting.ZaloPay.Key1, data));

                    using var client = new HttpClient();
                    var content = new FormUrlEncodedContent(param);
                    var response = client.PostAsync(_setting.ZaloPay.Url.QueryZpUrl(), content).Result;

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

    #endregion
}

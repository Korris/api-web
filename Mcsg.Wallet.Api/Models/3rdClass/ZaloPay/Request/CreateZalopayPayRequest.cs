using HD.ZaloPay.Helper.Crypto;
using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Request;

using Response;

public class CreateZalopayPayRequest
{
    public CreateZalopayPayRequest(string appId, string appUser, string appTime,
        long amount, string appTransId, string bankCode, string description, string callBackUrl, ZaloPayEmbedData embedData, List<ZaloPayItemData> items)
    {
        AppId = appId;
        AppUser = appUser;
        AppTime = appTime;
        Amount = amount;
        AppTransId = appTransId;
        BankCode = bankCode;
        Description = description;
        CallBackUrl = callBackUrl;
        EmbedData = embedData;
        Items = items;
    }

    public string AppId { get; set; }
    public string AppUser { get; set; } = string.Empty;
    public string AppTime { get; set; }
    public long Amount { get; set; }
    public string AppTransId { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public string CallBackUrl { get; set; } = string.Empty;
    public ZaloPayEmbedData EmbedData { get; set; } = new ZaloPayEmbedData();
    public List<ZaloPayItemData> Items { get; set; } = new List<ZaloPayItemData>();
    public string Mac { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public void MakeSignature(string key)
    {
        var data = AppId + "|" + AppTransId + "|" + AppUser + "|" + Amount.ToString() + "|"
            + AppTime + "|" + JsonConvert.SerializeObject(EmbedData) + "|" + JsonConvert.SerializeObject(Items);

        Mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key, data);
    }

    public Dictionary<string, string> GetContent()
    {
        var res = new Dictionary<string, string>
        {
            { "app_id", AppId },
            { "app_user", AppUser },
            { "app_time", AppTime },
            { "amount", Amount.ToString() },
            { "app_trans_id", AppTransId },
            { "description", Description },
            { "bank_code", "zalopayapp" },
            { "item", JsonConvert.SerializeObject(Items) },
            { "embed_data", JsonConvert.SerializeObject(EmbedData) },
            { "callback_url", CallBackUrl },
            { "mac", Mac }
        };

        return res;
    }

    public CreateZalopayPayResponse? GetLink(string paymentUrl)
    {
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(GetContent());
        var response = client.PostAsync(paymentUrl, content).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var responseData = JsonConvert
                .DeserializeObject<CreateZalopayPayResponse>(responseContent);

            return responseData;
        }
        else
        {
            return null;
        }
    }
}

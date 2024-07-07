using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Request;

public class ZaloPayCallBackData
{
    [JsonProperty("app_id")]
    public int AppId { get; set; }
    [JsonProperty("app_trans_id")]
    public string AppTransId { get; set; }
    [JsonProperty("app_time")]
    public string AppTime { get; set; }
    [JsonProperty("app_user")]
    public string AppUser { get; set; }
    [JsonProperty("amount")]
    public long Amount { get; set; }
    [JsonProperty("embed_data")]
    public ZaloPayEmbedData EmbedData { get; set; } = new ZaloPayEmbedData();
    [JsonProperty("item")]
    public List<ZaloPayItemData> Items { get; set; } = new List<ZaloPayItemData>();
    [JsonProperty("zp_trans_id")]
    public string ZaloPayTransId { get; set; } = string.Empty;
    [JsonProperty("server_time")]
    public string ServerTime { get; set; } = string.Empty;
    [JsonProperty("channel")]
    public string Channel { get; set; } = string.Empty;
    [JsonProperty("merchant_user_id")]
    public string MerchantUserId { get; set; } = string.Empty;
    [JsonProperty("user_fee_amount")]
    public long UserFeeAmount { get; set; }
    [JsonProperty("discount_amount")]
    public long DiscountAmount { get; set; }
}

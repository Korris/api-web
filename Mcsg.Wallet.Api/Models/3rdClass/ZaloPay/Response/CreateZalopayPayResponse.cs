using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Response;

using Common.Models;

public class CreateZalopayPayResponse : BaseZaloPayResponse
{
    [JsonProperty("zp_trans_token")]
    public string ZpTransToken { get; set; } = string.Empty;
    [JsonProperty("order_url")]
    public string OrderUrl { get; set; } = string.Empty;
    [JsonProperty("order_token")]
    public string OrderToken { get; set; } = string.Empty;
    [JsonProperty("qr_code")]
    public string QrCode { get; set; } = string.Empty;
}

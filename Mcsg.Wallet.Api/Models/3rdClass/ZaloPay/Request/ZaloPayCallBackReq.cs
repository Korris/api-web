using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Request;

using Constants.ZaloPay;

public class ZaloPayCallBackReq
{
    [JsonProperty("data")]
    public ZaloPayCallBackData Data { get; set; }
    [JsonProperty("mac")]
    public string Mac { get; set; }
    [JsonProperty("type")]
    public ZaloPayCallBackType Type { get; set; }
}

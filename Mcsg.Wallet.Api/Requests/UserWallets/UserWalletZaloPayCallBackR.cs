using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Requests;

using Constants.ZaloPay;
using Models._3rdClass.ZaloPay.Request;

public class UserWalletZaloPayCallBackR
{
    [JsonProperty("data")]
    public ZaloPayCallBackData Data { get; set; }
    [JsonProperty("mac")]
    public string Mac { get; set; }
    [JsonProperty("type")]
    public ZaloPayCallBackType Type { get; set; }
}

using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Response
{
    public class CallBackZaloPayResponse
    {
        [JsonProperty("return_code")]
        public int ReturnCode { get; set; }
        [JsonProperty("return_message")]
        public string ReturnMessage { get; set; } = string.Empty;
    }
}

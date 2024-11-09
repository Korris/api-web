using Newtonsoft.Json;

namespace Mcsg.Common.Models;

public class BaseZaloPayResponse
{
    [JsonProperty("return_code")]
    public int ReturnCode { get; set; }
    [JsonProperty("return_message")]
    public string ReturnMessage { get; set; } = string.Empty;
    [JsonProperty("sub_return_code")]
    public string SubReturnCode { get; set; } = string.Empty;
    [JsonProperty("sub_return_message")]
    public string SubReturnMessage { get; set; } = string.Empty;
}

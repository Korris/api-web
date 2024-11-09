using Newtonsoft.Json;

namespace Mcsg.Common.Models;

public class QueryZalopayPayResponse : BaseZaloPayResponse
{
    [JsonProperty("is_processing")]
    public string IsProcessing { get; set; } = string.Empty;
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;
    [JsonProperty("discount_amount")]
    public string DiscountAmount { get; set; } = string.Empty;
    [JsonProperty("zp_trans_id")]
    public string ZPTransId { get; set; } = string.Empty;
}

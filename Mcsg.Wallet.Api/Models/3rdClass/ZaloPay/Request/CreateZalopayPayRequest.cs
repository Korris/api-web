using HD.ZaloPay.Helper.Crypto;
using Mcsg.Wallet.Api.Constants;
using Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Response;
using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Request
{
    public class CreateZalopayPayRequest
    {
        public CreateZalopayPayRequest(int appId, string appUser, string appTime,
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
        public int AppId { get; set; }
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

            this.Mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key, data);
        }

        public Dictionary<string, string> GetContent()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            keyValuePairs.Add("app_id", AppId.ToString());
            keyValuePairs.Add("app_user", AppUser);
            keyValuePairs.Add("app_time", AppTime.ToString());
            keyValuePairs.Add("amount", Amount.ToString());
            keyValuePairs.Add("app_trans_id", AppTransId);
            keyValuePairs.Add("description", Description);
            keyValuePairs.Add("bank_code", SystemSettings.ZALO_PAY_BANK_CODE);
            keyValuePairs.Add("item", JsonConvert.SerializeObject(Items));
            keyValuePairs.Add("embed_data", JsonConvert.SerializeObject(EmbedData));
            keyValuePairs.Add("callback_url", CallBackUrl);
            keyValuePairs.Add("mac", Mac);

            return keyValuePairs;
        }

        public CreateZalopayPayResponse GetLink(string paymentUrl)
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
}

using Newtonsoft.Json;

namespace Mcsg.Lib.Common.Extensions;

using Helpers;
using Mcsg.Common.Core.Dtos;

public static class AffiliateExtension
{
    public static string ToAffiliateCode(this AffiliateDto data, string encryptKey)
    {
        if (data == null)
        {
            return "";
        }
        var dataStr = JsonConvert.SerializeObject(data);
        if (string.IsNullOrEmpty(encryptKey) || string.IsNullOrEmpty(dataStr))
        {
            return "";
        }
        var str = CryptoHelper.Encrypt(dataStr, encryptKey);
        return str;
    }

    public static AffiliateDto ToAffiliateData(this string code, string encryptKey)
    {
        if (string.IsNullOrEmpty(encryptKey) || string.IsNullOrEmpty(code))
        {
            return null;
        }
        var decryptCode = CryptoHelper.Decrypt(code, encryptKey);
        var data = JsonConvert.DeserializeObject<AffiliateDto>(decryptCode);
        return data;
    }
}

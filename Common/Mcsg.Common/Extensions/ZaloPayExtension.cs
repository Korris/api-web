namespace Mcsg.Lib.Common.Extensions;

public static class ZaloPayExtension
{
    public static string CreateZpUrl(this string url)
    {
        return !string.IsNullOrWhiteSpace(url) ? string.Format("{0}/create", url.TrimEnd('/')) : string.Empty;
    }
    public static string QueryZpUrl(this string url)
    {
        return !string.IsNullOrWhiteSpace(url) ? string.Format("{0}/query", url.TrimEnd('/')) : string.Empty;
    }
    public static string RefundZpUrl(this string url)
    {
        return !string.IsNullOrWhiteSpace(url) ? string.Format("{0}/refund", url.TrimEnd('/')) : string.Empty;
    }
    public static string QueryRefundZpUrl(this string url)
    {
        return !string.IsNullOrWhiteSpace(url) ? string.Format("{0}/query_refund", url.TrimEnd('/')) : string.Empty;
    }
}

using PuppeteerSharp;

namespace Mcsg.Lib.Common.Extensions
{
    public static class HtmlExtension
    {
        public static async Task<bool> IsConnected(this IPage page)
        {
            var connectionBlockContent = "No connection could be made";
            string pageContent = await page.GetContentAsync();
            return !pageContent.Contains(connectionBlockContent);
        }
        public static string HttpPrefix(this string url)
        {
            if (!url.Contains("http"))
            {
                string prefix = url.Substring(0, 2) == "//" ? "https:" : "https://";
                url = prefix + url;
            }
            return url;
        }
    }
}

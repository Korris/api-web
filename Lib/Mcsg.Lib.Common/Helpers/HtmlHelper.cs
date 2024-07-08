using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Mcsg.Lib.Common.Extensions;
using PuppeteerSharp;

namespace Mcsg.Lib.Common.Helpers
{
    public static class HtmlHelper
    {
        public static IElement? GetElementByHtmlSelector(string htmlContent, string selector)
        {
            if (string.IsNullOrEmpty(htmlContent) || string.IsNullOrEmpty(selector))
            {
                return null;
            }
            /// new
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });

            IDocument document = parser.ParseDocument(htmlContent);
            var element = document.QuerySelector(selector);

            return element;
        }
        public static async Task<IElementHandle[]> GetElementByHtmlXPath(string htmlContent, string xPath)
        {
            if (string.IsNullOrEmpty(htmlContent) || string.IsNullOrEmpty(xPath))
            {
                return null;
            }
            /// new
            var browser = await GetBrowserByPuppeteerSharp();
            using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(htmlContent);
                var element = await page.XPathAsync(xPath);

                return element;
            }
        }

        public static async Task<string> GetHtmlContentByAngleSharp(string url)
        {
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            return document.Source.Text;
        }
        public static async Task<IBrowser> GetBrowserByPuppeteerSharp()
        {
            // TODO
            BrowserFetcher browserFetcher = new();
            await browserFetcher.DownloadAsync();

            var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = false,
                    SlowMo = 10,
                    Args = new[] {
                  "--disable-gpu",
                  "--disable-dev-shm-usage",
                  "--disable-setuid-sandbox",
                  "--disable-dev-shm-usage",
                  "--no-sandbox"},
                    DefaultViewport = new ViewPortOptions { Width = 1440, Height = 900, DeviceScaleFactor = 2 }
                }
            );

            return browser;
        }
        public static async Task<IPage> GetPageByPuppeteerSharp(IBrowser browser, string url)
        {
            var page = await browser.NewPageAsync();
            page.DefaultTimeout = 300000;

            NavigationOptions defaultNavigationOptions = new() { WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle2 } };

            await page.GoToAsync(url, defaultNavigationOptions);

            return page;
        }
        public static async Task<string> GetPageContentAsync(IBrowser browser, string link)
        {
            if (browser == null)
            {
                return "";
            }

            var page = await HtmlHelper.GetPageByPuppeteerSharp(browser, link);
            var isConnected = await page.IsConnected();
            if (isConnected)
            {
                var overviewContent = await page.GetContentAsync();
                await page.CloseAsync();
                return overviewContent;
            }
            else
            {
                return "";
            }
        }
        public static string GetReadImageExtension(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "";
            }
            if (url.Contains("?"))
            {
                url = url.Split('?')[0];
            }
            string ext = System.IO.Path.GetExtension("@" + url);

            return ext;
        }
        public static string GetReadImageName(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "";
            }
            if (url.Contains("?"))
            {
                url = url.Split('?')[0];
            }
            string name = System.IO.Path.GetFileName("@" + url);

            return name;
        }
    }
}

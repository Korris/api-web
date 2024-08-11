using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Mcsg.Lib.Common.Helpers;

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

    public static async Task<string> GetHtmlContentByAngleSharp(string url)
    {
        var config = AngleSharp.Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url);

        return document.Source.Text;
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

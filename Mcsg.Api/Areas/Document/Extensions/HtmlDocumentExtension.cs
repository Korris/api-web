using HtmlAgilityPack;

namespace Mcsg.Api.Areas.Document.Extensions;

using Common.Core.Extensions;

public static class HtmlDocumentExtension
{
    public static string GetTitle(this HtmlDocument doc)
    {
        var ogNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
        if (ogNode != null)
        {
            return ogNode.GetAttributeValue("content", null);
        }

        var metaNameNode = doc.DocumentNode.SelectSingleNode("//meta[@name='title']");
        if (metaNameNode != null)
        {
            return metaNameNode.GetAttributeValue("content", null);
        }

        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        if (titleNode != null)
        {
            return titleNode.InnerText;
        }
        return string.Empty;
    }
    public static string GetDescription(this HtmlDocument doc)
    {
        var ogNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']");
        if (ogNode != null)
        {
            return ogNode.GetAttributeValue("content", null);
        }
        var metaNameNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
        if (metaNameNode != null)
        {
            return metaNameNode.GetAttributeValue("content", null);
        }
        return string.Empty;
    }
    public static string GetImage(this HtmlDocument doc, string url)
    {
        var ogNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
        if (ogNode != null)
        {
            return ogNode.GetAttributeValue("content", null);
        }

        //return doc.GetFavicon(url);

        return string.Empty;
    }
    public static string GetUrl(this HtmlDocument doc, string url)
    {
        var ogNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:url']");
        if (ogNode != null)
        {
            return ogNode.GetAttributeValue("content", null);
        }
        return url;
    }
    public static string GetFavicon(this HtmlDocument doc, string url)
    {
        var faviconLink = doc.DocumentNode.SelectSingleNode("//link[@rel='icon']");
        if (faviconLink != null)
        {
            var fvUrl = faviconLink.GetAttributeValue("href", null);
            return url.GetAbsolutePath(fvUrl);
        }

        var shortcutFaviconLink = doc.DocumentNode.SelectSingleNode("//link[@rel='shortcut icon']");
        if (shortcutFaviconLink != null)
        {
            var fvUrl = shortcutFaviconLink.GetAttributeValue("href", null);
            return url.GetAbsolutePath(fvUrl);
        }
        return string.Empty;
    }
}

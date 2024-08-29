using HtmlAgilityPack;

namespace Mcsg.Social.Api.Services;

using Common.SeedWork.Constants;
using Dtos;
using Extensions;
using Interfaces;

public class LinkPreviewService : ILinkPreviewService
{
    private readonly ILogger<LinkPreviewService> _logger;

    public LinkPreviewService(ILogger<LinkPreviewService> logger)
    {
        _logger = logger;
    }

    public async Task<MetaDataDto> GetMetaDataByUrl(string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Fetch the HTML content
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var htmlContent = await response.Content.ReadAsStringAsync();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Find meta tag
                var title = doc.GetTitle();
                var description = doc.GetDescription();
                var image = doc.GetImage(url);
                var ogUrl = doc.GetUrl(url);
                var uri = new Uri(url);

                return new MetaDataDto
                {
                    Title = title ?? "",
                    Description = description ?? "",
                    Image = image ?? "",
                    Url = ogUrl ?? "",
                    Domain = uri.Host
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(Error.E500, ex.Message);

            var uri = new Uri(url);
            var host = uri.Host;

            return new MetaDataDto
            {
                Title = host,
                Description = host,
                Image = "",
                Url = url,
                Domain = host
            };
        }
    }
}

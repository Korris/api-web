using HtmlAgilityPack;
using Mcsg.Social.Api.Extensions;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Common.Constants;

namespace Mcsg.Social.Api.Services
{
    public class LinkPreviewService : ILinkPreviewService
    {
        private readonly ILogger<LinkPreviewService> _logger;
        public LinkPreviewService(ILogger<LinkPreviewService> logger)
        {
            _logger = logger;
        }
        public MetaDataResponse GetMetaDataByUrl(string url)
        {
            try
            {
                // Load the HTML from the URL
                var web = new HtmlWeb();
                var doc = web.Load(url);

                // Find meta tag
                var title = doc.GetTitle();
                var description = doc.GetDescription();
                var image = doc.GetImage(url);
                var ogUrl = doc.GetUrl(url);
                var uri = new Uri(url);

                return new MetaDataResponse()
                {
                    Title = title ?? "",
                    Description = description ?? "",
                    Image = image ?? "",
                    Url = ogUrl ?? "",
                    Domain = uri.Host
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorCodes.ApiErrorCode, ex.Message);

                var uri = new Uri(url);
                string host = uri.Host;
                return new MetaDataResponse()
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
}

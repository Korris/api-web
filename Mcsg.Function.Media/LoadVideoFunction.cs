using Mcsg.Function.Media.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Mcsg.Function.Media
{
    public class LoadVideoFunction
    {
        private readonly ILogger _logger;
        private readonly IMediaService _mediaService;

        public LoadVideoFunction(ILoggerFactory loggerFactory, IMediaService mediaService)
        {
            _mediaService = mediaService;
            _logger = loggerFactory.CreateLogger<LoadVideoFunction>();
        }

        [Function("VideoViewer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "watch")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            if (req.Query.HasKeys())
            {
                if (req.Query["v"] != null)
                {
                    var url = _mediaService.GetRealUri(req.Query["v"]);
                    if (url.Contains("mp4"))
                    {
                        response.Headers.Add("Content-Type", "video/mp4");

                    }
                    else
                    {
                        response.Headers.Add("Content-Type", "video/webm");
                    }
                    response.Body = await _mediaService.ServeVideoAsync(req.Query["v"]);
                }
                if (req.Query["a"] != null)
                {
                    response.Headers.Add("Content-Type", "audio/mp3");
                    response.Body = await _mediaService.ServeVideoAsync(req.Query["a"]);
                }
            }

            response.Headers.Add("Content-Length", response.Body.Length.ToString());
            response.Headers.Add("Accept-Ranges", "bytes");
            response.Headers.Add("Cache-Control", "public, max-age=43800");
            return response;
        }
    }
}

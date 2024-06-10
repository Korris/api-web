using Mcsg.Function.Media.Services;

namespace Mcsg.Function.Media
{
    public class LoadImageFunction
    {
        private readonly ILogger _logger;
        private readonly IMediaService _mediaService;

        public LoadImageFunction(ILoggerFactory loggerFactory, IMediaService mediaService)
        {
            _mediaService = mediaService;
            _logger = loggerFactory.CreateLogger<LoadImageFunction>();
        }

        /*[Function("ImageViewer")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "image/jpeg");

            if (req.Query.HasKeys())
            {
                if (req.Query["i"] != null)
                {
                    var uri = _mediaService.GetRealUri(req.Query["i"]);
                    if (Path.GetExtension(uri).Equals(".gif", StringComparison.CurrentCultureIgnoreCase))
                    {
                        response.Headers.Remove("Content-Type");
                        response.Headers.Add("Content-Type", "image/gif");
                    }
                    response.Body = await _mediaService.ServeImageAsync(uri);
                }
                if (req.Query["p"] != null)
                {
                    response.Body = await _mediaService.ServePublicImageAsync(req.Query["p"]);
                }
            }
            response.Headers.Add("Cache-Control", "public, max-age=43800");

            return response;
        }*/
    }
}

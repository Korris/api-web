using Mcsg.Lib.Common.Helpers;
using Mcsg.Media.Tool.Models;
using Microsoft.Extensions.Configuration;

namespace Mcsg.Media.Tool.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _configuration;
        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> AddVideoNotificationAsync(VideoNotificationReq req)
        {
            var baseUrl = _configuration["RealTimeServiceSettings:BaseUrl"];
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/video");

            var url = urlBuilder.ToString();

            var response = await HttpHelper.MakePostRequest(url, req);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Send Video Noti Video Id: {0} - Video HashId : {1} - Response {2}", req.Id, req.HashId, responseContent);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

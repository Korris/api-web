namespace Mcsg.Media.Tool.Services
{
    using Lib.Common.Helpers;
    using Models;

    public class NotificationService
    {
        public NotificationService()
        {
        }

        public async Task<bool> AddVideoNotificationAsync(VideoNotificationReq req, string baseUrl)
        {
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

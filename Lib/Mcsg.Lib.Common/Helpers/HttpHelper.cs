using Mcsg.Lib.Common.Models;
using System.Net;
using System.Text;

namespace Mcsg.Lib.Common.Helpers
{
    public class HttpHelper
    {
        public static async Task<HttpResponseMessage> MakePostRequest(string apiUrl, object data)
        {
            using (HttpClient client = new HttpClient())
            {
                // Convert the data to JSON
                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                // Create the HTTP content with the JSON data
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Make a POST request to the Web API
                return await client.PostAsync(apiUrl, content);
            }
        }
        public static ImageStreamSize GetImageStream(string imageUrl, string referer = "")
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("Referer", !string.IsNullOrWhiteSpace(referer) ? referer : "nettruyen");
                var imageData = client.DownloadData(imageUrl);
                Stream stream = new MemoryStream(imageData);

                using SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(imageData);

                if (image == null)
                {
                    return null;
                }
                else
                {
                    return new ImageStreamSize()
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Length = imageData.LongLength,
                        Stream = stream
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

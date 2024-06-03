using Mcsg.Lib.Common.Constants;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Mcsg.Lib.Common.Helpers
{
    public static class UrlHelper
    {
        public static string GetAbsolutePath(string url, string relativePath)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri absoluteUri))
            {
                return null; // Invalid URL
            }

            if (string.IsNullOrEmpty(relativePath))
            {
                return absoluteUri.AbsoluteUri;
            }

            Uri resultUri;

            if (relativePath.StartsWith("/"))
            {
                resultUri = new Uri(absoluteUri, relativePath);
            }
            else if (relativePath.StartsWith("./"))
            {
                string combinedPath = absoluteUri.AbsolutePath;
                combinedPath = combinedPath.Substring(0, combinedPath.LastIndexOf('/')); // Remove the last segment
                combinedPath = combinedPath.TrimEnd('/'); // Remove trailing slashes

                resultUri = new Uri(absoluteUri, new Uri(combinedPath + '/' + relativePath, UriKind.Relative));
            }
            else
            {
                // If relativePath does not start with "/", "./", or "../", it's considered an absolute path.
                resultUri = new Uri(relativePath, UriKind.RelativeOrAbsolute);
            }

            return resultUri.AbsoluteUri;
        }
        public static string GetMediaPath(string baseUrl, string name, string url)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new FormatException(ErrorCodes.InvalidFile);
            }

            string fileExtension = Path.GetExtension(name);
            string mediaPath = "";
            if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                mediaPath = string.Format(MediaConfig.AudioUrlPath, url);
            }
            else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                mediaPath = string.Format(MediaConfig.ImageUrlPath, url);
            }
            else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                mediaPath = string.Format(MediaConfig.VideoUrlPath, url);
            }
            Uri baseUri = new Uri(baseUrl);
            Uri mediaUri = new Uri(baseUri, mediaPath);
            return mediaUri.AbsoluteUri;
        }
        public static string CreateMediaUrl(string text, string encryptKey)
        {
            return HttpUtility.UrlEncode(CryptoHelper.Encrypt(text, encryptKey));
        }

        public static string GetPublicImageUrl(IConfiguration configuration, string mediaName)
        {
            if (string.IsNullOrWhiteSpace(mediaName))
                return string.Empty;
            return string.Format(Path.Combine(configuration["FileSettings:MediaUrl"], MediaConfig.PublicImageUrlPath), mediaName);
        }

        public static string GetStorageDomain(string accountStorageName)
        {
            return $"https://{accountStorageName}.blob.core.windows.net/";
        }

        public static string GetShareUrlFromStorage(this Uri originalUrl, string accountStorageName)
        {
            return originalUrl.ToString().Replace(GetStorageDomain(accountStorageName), "");
        }

        public static string CreateCdnMediaUrl(string url, IConfiguration configuration)
        {
            return Path.Combine(configuration["FileSettings:MediaCDNUrl"], url);
        }

    }
}

using System.Web;

namespace Mcsg.Lib.Common.Helpers
{
    using Lib.Common.Constants;

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

            return $"{baseUrl}/{mediaPath}";
        }

        public static string CreateMediaUrl(string text, string encryptKey)
        {
            return HttpUtility.UrlEncode(text);
        }

        public static string GetPublicImageUrl(string mediaApiUrl, string mediaName)
        {
            if (string.IsNullOrWhiteSpace(mediaName))
            {
                return string.Empty;
            }

            var url = string.Format(MediaConfig.PublicImageUrlPath, mediaName);
            return $"{mediaApiUrl}/{url}";
        }
    }
}

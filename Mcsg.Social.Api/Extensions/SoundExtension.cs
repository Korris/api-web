namespace Mcsg.Social.Api.Extensions
{
    using Lib.Common.Helpers;

    public static class SoundExtension
    {
        public static string ToDuration(this int value)
        {
            TimeSpan ts = TimeSpan.FromSeconds(value);
            return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
        public static string ToMediaPath(this string url, string fileName, string mediaApiUrl)
        {
            return UrlHelper.GetMediaPath(mediaApiUrl, fileName, url);
        }
        public static string ToAudioPath(this string url, string mediaApiUrl)
        {
            return url.ToMediaPath(".mp3", mediaApiUrl);
        }
        public static string ToImagePath(this string url, string mediaApiUrl)
        {
            return url.ToMediaPath(".jpg", mediaApiUrl);
        }
        public static string ToPublicImageUrl(this string fileName, string mediaApiUrl)
        {
            return UrlHelper.GetPublicImageUrl(mediaApiUrl, fileName);
        }
    }
}

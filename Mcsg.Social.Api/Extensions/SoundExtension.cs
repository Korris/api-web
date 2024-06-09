using Mcsg.Social.Api.Helper;
using Mcsg.Lib.Common.Helpers;

namespace Mcsg.Social.Api.Extensions
{
    public static class SoundExtension
    {
        public static string ToDuration(this int value)
        {
            TimeSpan ts = TimeSpan.FromSeconds(value);
            return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
        public static string ToMediaPath(this string url, string fileName)
        {
            var baseMediaUrl = AppSettingsProvider.Configuration["FileSettings:MediaUrl"];
            return UrlHelper.GetMediaPath(baseMediaUrl, fileName, url);
        }
        public static string ToAudioPath(this string url)
        {
            return url.ToMediaPath(".mp3");
        }
        public static string ToImagePath(this string url)
        {
            return url.ToMediaPath(".jpg");
        }
        public static string ToPublicImageUrl(this string fileName)
        {
            return UrlHelper.GetPublicImageUrl(AppSettingsProvider.Configuration, fileName);
        }
    }
}

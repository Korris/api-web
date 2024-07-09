namespace Mcsg.Lib.Common.Extensions
{
    using Constants;
    using Mcsg.Common.Core.Enums;

    public static class ResourceExtensions
    {
        public static ResourceType GetResourceType(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new FormatException(ErrorCodes.InvalidFile);
            }

            string fileExtension = Path.GetExtension(name);
            if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return ResourceType.Audio;
            }
            else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return ResourceType.Image;
            }
            else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return ResourceType.Video;
            }
            else
            {
                return ResourceType.Other;
            }
        }
        public static string CreateCdnMediaUrl(this string url, string mediaCdnUrl)
        {
            return Path.Combine(mediaCdnUrl, url);
        }
    }
}

using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Lib.Common.Extensions
{
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
                return ResourceType.AUDIO;
            }
            else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return ResourceType.IMAGE;
            }
            else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return ResourceType.VIDEO;
            }
            else
            {
                return ResourceType.OTHER;
            }
        }
        public static string CreateCdnMediaUrl(this string url, string mediaCdnUrl)
        {
            return Path.Combine(mediaCdnUrl, url);
        }
    }
}

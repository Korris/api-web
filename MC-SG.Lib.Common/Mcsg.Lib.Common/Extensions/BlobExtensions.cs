using Mcsg.Lib.Common.Constants;

namespace Mcsg.Lib.Common.Extensions
{
    public static class BlobExtensions
    {
        public static string GetTempBlobName(this string fileName, string parentFolder)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new FormatException(ErrorCodes.InvalidFile);
            }
            if (string.IsNullOrWhiteSpace(parentFolder))
            {
                throw new FormatException(ErrorCodes.InvalidParentFolder);
            }

            return String.Format("{0}/{1}/{2}", parentFolder, FileLocations.Temp, fileName);
        }
        public static string GetMediaBlobName(this string fileName, string parentFolder)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new FormatException(ErrorCodes.InvalidFile);
            }
            if (string.IsNullOrWhiteSpace(parentFolder))
            {
                throw new FormatException(ErrorCodes.InvalidParentFolder);
            }

            var folder = fileName.GetFileLocation();
            return String.Format("{0}/{1}/{2}", parentFolder, folder, fileName);
        }
    }
}

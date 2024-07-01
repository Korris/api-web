using ImageMagick;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mcsg.Lib.Common.Extensions
{
    using Constants;
    using Mcsg.Common.SeedWork.Extensions;
    using Models;

    public static class FileExtensions
    {
        public static string GetFileLocation(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(file.FileName);
            if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return FileLocations.Audio;
            }
            else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return FileLocations.Image;
            }
            else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return FileLocations.Video;
            }
            else
            {
                return FileLocations.Other;
            }
        }
        public static string GetFileLocation(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(fileName);
            if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return FileLocations.Audio;
            }
            else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return FileLocations.Image;
            }
            else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return FileLocations.Video;
            }
            else
            {
                return FileLocations.Other;
            }
        }
        public static string GetHashName(this IFormFile file, string hashId = "")
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }
            hashId = !string.IsNullOrWhiteSpace(hashId) ? hashId : ResourcesDefinition.HashLength.GetRandomString();
            string extension = Path.GetExtension(file.FileName);
            return hashId + extension;
        }
        public static bool IsImage(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(file.FileName);

            return FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsImageType(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            // Check if the content type is in the allowed list
            return Array.Exists(FileTypes.Images, type => type.Equals(file.ContentType, StringComparison.OrdinalIgnoreCase));
        }
        public static bool IsGif(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(file.FileName);

            return ".gif".Contains(fileExtension, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsImageWithOutGif(this IFormFile file)
        {
            var imgArr = FileExt.Images.Where(x => x != ".gif").Select(x => x).ToArray();
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(file.FileName);

            return imgArr.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsAudio(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(file.FileName);

            return FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsVideo(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new FormatException(ErrorMessage.InvalidFile);
            }

            string fileExtension = Path.GetExtension(file.FileName);

            return FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
        public static string ToJpg(this string name)
        {
            string extension = Path.GetExtension(name);
            return name.Replace(extension, ".jpg");
        }
        public static CompressImage CompressAndConvertToJpeg(this IFormFile file, int quality)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var magickImage = new MagickImage(stream))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            magickImage.Format = MagickFormat.Jpeg;
                            magickImage.Quality = quality;
                            magickImage.Write(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            using (var image = SixLabors.ImageSharp.Image.Load(memoryStream))
                            {
                                var output = new MemoryStream();
                                image.Save(output, new JpegEncoder { Quality = quality });
                                output.Seek(0, SeekOrigin.Begin);
                                var compressedFile = new FormFile(output, 0, output.Length, file.Name, file.FileName.ToJpg());
                                return new CompressImage()
                                {
                                    Image = compressedFile,
                                    Width = image.Width,
                                    Height = image.Height
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static ImageRatio GetRatio(this IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var image = SixLabors.ImageSharp.Image.Load(stream))
                    {
                        return new ImageRatio()
                        {
                            Width = image.Width,
                            Height = image.Height
                        };
                    }
                }
            }
            return null;
        }
        public static bool IsGifAnimated(this IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream))
                        {
                            if (ImageAnimator.CanAnimate(image))
                            {
                                // Check the number of frames in the image
                                int frameCount = image.GetFrameCount(FrameDimension.Time);
                                return frameCount > 1;
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}

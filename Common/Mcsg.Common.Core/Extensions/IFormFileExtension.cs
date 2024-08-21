using ImageMagick;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mcsg.Common.Core.Extensions;

using Constants;
using Dtos;
using SeedWork.Extensions;
using static SeedWork.Constants.Message;

/// <summary>
/// IFormFile extension for using [this IFormFile] only
/// </summary>
public static class IFormFileExtension
{
    /// <summary>
    /// GetHashName
    /// </summary>
    /// <param name="file"></param>
    /// <param name="hashId"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static string GetHashName(this IFormFile file, string hashId = "")
    {
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        hashId = !string.IsNullOrWhiteSpace(hashId) ? hashId : Setting.ResourceConfig.HashLength.GetRandomString();
        var extension = Path.GetExtension(file.FileName);
        return hashId + extension;
    }

    /// <summary>
    /// IsImage
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static bool IsImage(this IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        string fileExtension = Path.GetExtension(file.FileName);

        return Setting.FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// IsImageType
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static bool IsImageType(this IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        // Check if the content type is in the allowed list
        return Array.Exists(Setting.FileType.Images, type => type.Equals(file.ContentType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// IsGif
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static bool IsGif(this IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        string fileExtension = Path.GetExtension(file.FileName);

        return ".gif".Contains(fileExtension, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// IsImageWithOutGif
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static bool IsImageWithOutGif(this IFormFile file)
    {
        var imgArr = Setting.FileExt.Images.Where(x => x != ".gif").Select(x => x).ToArray();
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        string fileExtension = Path.GetExtension(file.FileName);

        return imgArr.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// IsAudio
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static bool IsAudio(this IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        string fileExtension = Path.GetExtension(file.FileName);

        return Setting.FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// IsVideo
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static bool IsVideo(this IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new FormatException(M112);
        }

        string fileExtension = Path.GetExtension(file.FileName);

        return Setting.FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Compress and convert to JPEG
    /// </summary>
    /// <param name="file">File</param>
    /// <param name="quality">Quality</param>
    /// <param name="dpi">DPI adjustment</param>
    /// <returns>Returns the result</returns>
    public static CompressImage? CompressAndConvertToJpeg(this IFormFile file, int quality, int dpi = 72)
    {
        if (file != null && file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                using (var magickImage = new MagickImage(stream))
                {
                    magickImage.Density = new Density(dpi);

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
                            var compressedFile = new FormFile(output, 0, output.Length, file.Name, file.FileName);

                            return new CompressImage
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

    /// <summary>
    /// GetRatio
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static ImageRatio? GetRatio(this IFormFile file)
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

    /// <summary>
    /// IsGifAnimated
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsGifAnimated(this IFormFile file)
    {
        try
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var image = Image.FromStream(stream))
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

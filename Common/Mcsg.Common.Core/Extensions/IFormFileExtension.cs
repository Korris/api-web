using ImageMagick;
using Microsoft.AspNetCore.Http;
using OpenMcdf;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;

namespace Mcsg.Common.Core.Extensions;

using Constants;
using Dtos;
using SeedWork.Exceptions;
using SeedWork.Extensions;
using static SeedWork.Constants.Error;

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
            throw new FormatException(nameof(E112));
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
            throw new FormatException(nameof(E112));
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
            throw new FormatException(nameof(E112));
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
            throw new FormatException(nameof(E112));
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
            throw new FormatException(nameof(E112));
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
            throw new FormatException(nameof(E112));
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
            throw new FormatException(nameof(E112));
        }

        return file.OpenReadStream().IsVideo();
    }

    /// <summary>
    /// Compress image and upload
    /// </summary>
    /// <param name="file"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static CompressImage? CompressAndConvertToJpeg(this IFormFile file, uint width, uint height, uint quality)
    {
        if (file == null || file.Length <= 0)
        {
            return null;
        }

        try
        {
            var stream = file.OpenReadStream();
            var output = stream.ResizeImage(width, height, quality);
            if (output == null)
            {
                return null;
            }

            return new CompressImage
            {
                Image = new FormFile(output, 0, output.Length, file.Name, file.FileName)
            };
        }
        catch
        {
            throw new BadRequestException(nameof(E210), E210);
        }
    }

    /// <summary>
    /// Compress and convert to JPEG
    /// </summary>
    /// <param name="file">File</param>
    /// <param name="quality">Quality</param>
    /// <param name="dpi">DPI adjustment</param>
    /// <returns>Returns the result</returns>
    public static CompressImage? CompressAndConvertToJpeg(this IFormFile file, uint quality, int dpi = 72)
    {
        if (file != null && file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                try
                {
                    using (var magickImage = new MagickImage(stream))
                    {
                        magickImage.Density = new Density(dpi);

                        using (var ms = new MemoryStream())
                        {
                            magickImage.Format = MagickFormat.Jpeg;
                            magickImage.Quality = quality;
                            magickImage.Write(ms);
                            ms.Seek(0, SeekOrigin.Begin);

                            using (var image = SixLabors.ImageSharp.Image.Load(ms))
                            {
                                var output = new MemoryStream();
                                image.Save(output, new JpegEncoder { Quality = (int)quality });
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
                catch
                {
                    throw new BadRequestException(nameof(E210), E210);
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

    /// <summary>
    /// IsDocument
    /// </summary>
    /// <param name="file">File</param>
    /// <returns>Return the result</returns>
    public static bool IsDocument(this IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return false;
        }

        var ext = Path.GetExtension(file.FileName).ToUpper();
        using var fs = file.OpenReadStream();
        if (!fs.IsDocument(ext))
        {
            return false;
        }

        if (ext == ".DOC" || ext == ".PPT")
        {
            using var compoundFile = new CompoundFile(fs);

            // DOC
            if (compoundFile.RootStorage.TryGetStream("WordDocument", out _))
            {
                return ext == ".DOC";
            }

            // PPT
            if (compoundFile.RootStorage.TryGetStream("PowerPoint Document", out _))
            {
                return ext == ".PPT";
            }
        }

        if (ext == ".DOCX" || ext == ".PPTX")
        {
            try
            {
                using var archive = new ZipArchive(fs, ZipArchiveMode.Read, true);

                // DOCX
                if (ext == ".DOCX")
                {
                    return archive.GetEntry("word/document.xml") != null;
                }

                // PPTX
                if (ext == ".PPTX")
                {
                    return archive.GetEntry("ppt/presentation.xml") != null;
                }
            }
            catch { }
        }

        return true;
    }
}

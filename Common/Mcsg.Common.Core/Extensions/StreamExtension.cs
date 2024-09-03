#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using ImageMagick;
using SixLabors.ImageSharp.Formats.Jpeg;
using SkiaSharp;

namespace Mcsg.Common.Core.Extensions;

/// <summary>
/// StreamExtension extension for using [this StreamExtension] only
/// </summary>
public static class StreamExtension
{
    #region -- Methods --

    /// <summary>
    /// Resize image
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="width">New width</param>
    /// <param name="height">New height</param>
    /// <returns>Return the result</returns>
    public static Stream ResizeImage(this Stream fs, int width, int height)
    {
        using (var original = SKBitmap.Decode(fs))
        {
            // Create a new bitmap with the new size
            using (var resized = new SKBitmap(width, height))
            using (var canvas = new SKCanvas(resized))
            {
                // Draw the original bitmap into the resized bitmap
                var paint = new SKPaint
                {
                    FilterQuality = SKFilterQuality.High
                };

                canvas.DrawBitmap(original, new SKRect(0, 0, width, height), paint);

                // Encode the resized image to a memory stream
                using (var image = SKImage.FromBitmap(resized))
                {
                    var data = image.Encode(SKEncodedImageFormat.Png, 100);
                    var ms = new MemoryStream();
                    data.SaveTo(ms);

                    return ms;
                }
            }
        }
    }

    /// <summary>
    /// Resize image
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="width">New width</param>
    /// <param name="height">New height</param>
    /// <param name="quality">Quality</param>
    /// <returns>Return the result</returns>
    public static Stream? ResizeImage(this Stream? fs, int width, int height, int quality)
    {
        if (fs == null)
        {
            return null;
        }

        // Ensure the Stream's position is at the beginning
        fs.Position = 0;

        using (var magickImage = new MagickImage(fs))
        {
            magickImage.Density = new Density(72);
            magickImage.SetBitDepth(24);

            if (magickImage.BaseHeight <= height || magickImage.BaseWidth <= width)
            {
                return fs;
            }

            magickImage.Resize(new MagickGeometry
            {
                Width = width,
                Height = height,
                FillArea = true // Resize to fill the area
            });

            // Optionally crop the image to ensure it fits the exact dimensions
            magickImage.Crop(width, height, Gravity.Center);
            using (var ms = new MemoryStream())
            {
                magickImage.Format = MagickFormat.Jpeg;
                magickImage.Quality = quality;
                magickImage.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);

                using (var image = SixLabors.ImageSharp.Image.Load(ms))
                {
                    var output = new MemoryStream();
                    image.Save(output, new JpegEncoder { Quality = quality });
                    output.Seek(0, SeekOrigin.Begin);
                    return output;
                }
            }
        }
    }

    #endregion
}

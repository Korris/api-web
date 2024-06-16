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
    /// <param name="height">new height</param>
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

    #endregion
}

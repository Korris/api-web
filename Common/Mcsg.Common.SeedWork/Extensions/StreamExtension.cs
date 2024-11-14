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

using System.Text;

namespace Mcsg.Common.SeedWork.Extensions;

using SeedWork.Enums;

/// <summary>
/// StreamExtension extension for using [this StreamExtension] only
/// </summary>
public static class StreamExtension
{
    #region -- Methods --

    /// <summary>
    /// Get media type
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <returns>Return the result</returns>
    public static MediaType GetMediaType(this Stream fs)
    {
        MediaType res;

        var buffer = new byte[12];
        fs.Read(buffer, 0, 12);

        if (buffer.IsImage())
        {
            res = MediaType.Image;
        }
        else if (buffer.IsVideo())
        {
            res = MediaType.Video;
        }
        else
        {
            res = MediaType.Unknown;
        }

        return res;
    }

    /// <summary>
    /// Checks if the given stream is an image by inspecting its file signature (header).
    /// </summary>
    /// <param name="fs">The input stream to check.</param>
    /// <returns>Returns true if the stream represents an image in a supported format; otherwise, false.</returns>
    public static bool IsImage(this Stream? fs)
    {
        if (fs == null)
        {
            return false;
        }

        var buffer = new byte[12];
        fs.Read(buffer, 0, buffer.Length);

        return buffer.IsImage();
    }

    /// <summary>
    /// Checks if the given stream is a video by inspecting its file signature (header).
    /// </summary>
    /// <param name="fs">The input stream to check.</param>
    /// <returns>Returns true if the stream represents a video in a supported format; otherwise, false.</returns>
    public static bool IsVideo(this Stream? fs)
    {
        if (fs == null)
        {
            return false;
        }

        var buffer = new byte[12];
        fs.Read(buffer, 0, buffer.Length);

        return buffer.IsVideo();
    }

    /// <summary>
    /// Checks if the given stream is a video by inspecting its file signature (header).
    /// </summary>
    /// <param name="fs">The input stream to check.</param>
    /// <param name="extension">Extension</param>
    /// <returns>Returns true if the stream represents a video in a supported format; otherwise, false.</returns>
    public static bool IsDocument(this Stream? fs, string? extension)
    {
        if (fs == null)
        {
            return false;
        }

        var buffer = new byte[12];
        fs.Read(buffer, 0, buffer.Length);

        return buffer.IsDocument(extension);
    }

    /// <summary>
    /// Convert stream to string
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <returns>Return the result</returns>
    public static string ToString(this Stream? fs)
    {
        if (fs == null)
        {
            return string.Empty;
        }

        var bufferSize = 1024; // 1 KB buffer size
        var buffer = new byte[bufferSize];
        var stringBuilder = new StringBuilder();

        // Ensure the position is at the beginning of the Stream
        fs.Position = 0;

        int bytesRead;
        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
        {
            // Convert the read bytes to a string and append to the StringBuilder
            stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Convert stream to file
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="filePath">File path</param>
    public static void ToFile(this Stream? fs, string filePath)
    {
        if (fs == null)
        {
            return;
        }

        // Ensure the Stream's position is at the beginning
        fs.Position = 0;

        // Write the Stream content to a file
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            fs.CopyTo(fileStream);
        }
    }

    #endregion
}

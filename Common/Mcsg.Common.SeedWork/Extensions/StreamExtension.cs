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

namespace Mcsg.Common.SeedWork.Extensions;

using Common.SeedWork.Enums;

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

    #endregion
}

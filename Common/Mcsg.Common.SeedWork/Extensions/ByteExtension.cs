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

/// <summary>
/// Byte extension for using [this byte] only
/// </summary>
public static class ByteExtension
{
    #region -- Methods --

    /// <summary>
    /// Random password
    /// </summary>
    /// <param name="length">Specify the length of the password</param>
    /// <returns>Return the password</returns>
    public static string RandomPassword(this byte length)
    {
        var chars = new byte[62];
        for (int i = 0; i < 10; i++)
        {
            chars[i] = (byte)(i + 48); // '0' to '9'
        }
        for (int i = 0; i < 26; i++)
        {
            chars[i + 10] = (byte)(i + 65); // 'A' to 'Z'
        }
        for (int i = 0; i < 26; i++)
        {
            chars[i + 36] = (byte)(i + 97); // 'a' to 'z'
        }

        var rdm = new Random();
        var res = new byte[length];
        for (int i = 0; i < length; i++)
        {
            res[i] = chars[rdm.Next(62)];
        }

        return Encoding.ASCII.GetString(res);
    }

    /// <summary>
    /// Is image
    /// </summary>
    /// <param name="buffer">Buffer</param>
    /// <returns>Return the result</returns>
    public static bool IsImage(this byte[] buffer)
    {
        // Check for null or insufficient length
        if (buffer == null || buffer.Length < 8)
        {
            return false;
        }

        // JPEG/JFIF
        if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
        {
            return true;
        }

        // PNG
        if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
        {
            return true;
        }

        // GIF
        if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46)
        {
            return true;
        }

        // BMP
        if (buffer[0] == 0x42 && buffer[1] == 0x4D)
        {
            return true;
        }

        // HEIF/HEIC
        if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x00 && buffer[3] == 0x18 &&
            buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
        {
            if (buffer.Length >= 16)
            {
                var brand = Encoding.ASCII.GetString(buffer, 8, 4);

                if (brand == "heic" || brand == "heix" || brand == "hevc" || brand == "mif1" ||
                    brand == "msf1" || brand == "avci" || brand == "hevx" || brand == "heim" ||
                    brand == "heis" || brand == "avif")
                {
                    return true;
                }

                // MP4
                if (brand == "mp42" || brand == "isom" || brand == "mp41" || brand == "avc1" ||
                    brand == "dash" || brand == "qt")
                {
                    return false;
                }
            }
        }

        // WebP
        if (buffer.Length >= 12 &&
            buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
            buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Is video
    /// </summary>
    /// <param name="buffer">Buffer</param>
    /// <returns>Return the result</returns>
    public static bool IsVideo(this byte[] buffer)
    {
        // MP4
        if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
        {
            return true;
        }

        // AVI
        if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46)
        {
            return true;
        }

        // MKV
        if (buffer[0] == 0x1A && buffer[1] == 0x45 && buffer[2] == 0xDF && buffer[3] == 0xA3)
        {
            return true;
        }

        // MOV
        if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Is document
    /// </summary>
    /// <param name="buffer">Buffer</param>
    /// <param name="extension">Extension</param>
    /// <returns>Return the result</returns>
    public static bool IsDocument(this byte[] buffer, string? extension)
    {
        var ext = (extension + "").ToUpper();

        // PDF
        if (ext == ".PDF" && buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
        {
            return true;
        }

        // PPT & DOC
        if ((ext == ".PPT" || ext == ".DOC") && buffer[0] == 0xD0 && buffer[1] == 0xCF && buffer[2] == 0x11 && buffer[3] == 0xE0)
        {
            return true;
        }

        // PPTX & DOCX
        if ((ext == ".PPTX" || ext == ".DOCX") && buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04)
        {
            return true;
        }

        return false;
    }

    #endregion
}

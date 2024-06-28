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
/// Int extension for using [this int] only
/// </summary>
public static class IntExtension
{
    #region -- Methods --

    /// <summary>
    /// Get random string
    /// </summary>
    /// <param name="length">Length</param>
    /// <returns>Return the result</returns>
    public static string GetRandomString(this int length)
    {
        var letters = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            var c = letters[random.Next(0, letters.Length)];
            builder.Append(c);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Generate OTP
    /// </summary>
    /// <param name="length">Length</param>
    /// <returns>Return the result</returns>
    public static string GenerateOtp(this int length)
    {
        var letters = "0123456789";
        var random = new Random();

        var otp = new char[length];
        for (var i = 0; i < length; i++)
        {
            otp[i] = letters[random.Next(letters.Length)];
        }

        return new string(otp);
    }

    #endregion
}

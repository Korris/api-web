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

using Enums;

/// <summary>
/// Double extension for using [this double] only
/// </summary>
public static class DoubleExtension
{
    #region -- Methods --

    /// <summary>
    /// From kilobytes
    /// </summary>
    /// <param name="kilobytes"></param>
    /// <returns>Return the result</returns>
    public static double FromKilobytes(this double kilobytes) => From(kilobytes, ByteType.Kilobytes);

    /// <summary>
    /// From megabytes
    /// </summary>
    /// <param name="megabytes"></param>
    /// <returns>Return the result</returns>
    public static double FromMegabytes(this double megabytes) => From(megabytes, ByteType.Megabytes);

    /// <summary>
    /// From gigabytes
    /// </summary>
    /// <param name="gigabytes"></param>
    /// <returns>Return the result</returns>
    public static double FromGigabytes(this double gigabytes) => From(gigabytes, ByteType.Gigabytes);

    /// <summary>
    /// From terabytes
    /// </summary>
    /// <param name="terabytes"></param>
    /// <returns>Return the result</returns>
    public static double FromTerabytes(this double terabytes) => From(terabytes, ByteType.Terabytes);

    /// <summary>
    /// To kilobytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return the result</returns>
    public static double ToKilobytes(this double bytes) => To(bytes, ByteType.Kilobytes);

    /// <summary>
    /// To megabytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return the result</returns>
    public static double ToMegabytes(this double bytes) => To(bytes, ByteType.Megabytes);

    /// <summary>
    /// To gigabytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return the result</returns>
    public static double ToGigabytes(this double bytes) => To(bytes, ByteType.Gigabytes);

    /// <summary>
    /// To terabytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return the result</returns>
    public static double ToTerabytes(this double bytes) => To(bytes, ByteType.Terabytes);

    /// <summary>
    /// To
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="kind">Kind</param>
    /// <returns>Return the result</returns>
    private static double To(double value, ByteType kind) => value / Math.Pow(1024, (int)kind);

    /// <summary>
    /// From
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="kind">Kind</param>
    /// <returns>Return the result</returns>
    private static double From(double value, ByteType kind) => value * Math.Pow(1024, (int)kind);

    #endregion
}

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
    /// From bytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double FromBytes(this double value) => From(value, ByteType.Bytes);

    /// <summary>
    /// From kilobytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double FromKilobytes(this double value) => From(value, ByteType.Kilobytes);

    /// <summary>
    /// From megabytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double FromMegabytes(this double value) => From(value, ByteType.Megabytes);

    /// <summary>
    /// From gigabytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double FromGigabytes(this double value) => From(value, ByteType.Gigabytes);

    /// <summary>
    /// From terabytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double FromTerabytes(this double value) => From(value, ByteType.Terabytes);

    /// <summary>
    /// To bytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double ToBytes(this double value) => To(value, ByteType.Bytes);

    /// <summary>
    /// To kilobytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double ToKilobytes(this double value) => To(value, ByteType.Kilobytes);

    /// <summary>
    /// To megabytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double ToMegabytes(this double value) => To(value, ByteType.Megabytes);

    /// <summary>
    /// To gigabytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double ToGigabytes(this double value) => To(value, ByteType.Gigabytes);

    /// <summary>
    /// To terabytes
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Return the result</returns>
    public static double ToTerabytes(this double value) => To(value, ByteType.Terabytes);

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

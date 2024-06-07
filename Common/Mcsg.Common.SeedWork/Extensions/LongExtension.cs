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

/// <summary>
/// Long extension for using [this long] only
/// </summary>
public static class LongExtension
{
    #region -- Methods --

    /// <summary>
    /// Create a serial number with format [prefix][A-Z][000001-9999999]
    /// </summary>
    /// <param name="num">Number</param>
    /// <param name="prefix">Prefix</param>
    /// <returns>Return the result</returns>
    public static string ToSerialNumber(this ulong num, string prefix)
    {
        string? res;

        if (num <= 9999999)
        {
            res = prefix + "A" + num.ToString("000000");
        }
        else if (num <= 19999998)  // Assuming you stop at Z for this example
        {
            char letter = (char)('A' + (num - 9999999) % 26);
            res = prefix + +letter + (num - 9999999).ToString("000000");
        }
        else
        {
            res = "OVERFLOW";
        }

        return res;
    }

    #endregion
}

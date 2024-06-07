#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using System.Reflection;
using System.Text.RegularExpressions;

namespace Mcsg.Common.SeedWork.Extensions;

using Enums;

/// <summary>
/// Object extension for using [this object] only
/// </summary>
public static class ObjectExtension
{
    #region -- Methods --

    /// <summary>
    /// Get value if a property exist in object
    /// </summary>
    /// <param name="o">Object</param>
    /// <param name="p">Property name</param>
    /// <returns>Return the result</returns>
    public static object? GetPropertyValue(this object o, string p)
    {
        object? res = null;

        if (o != null)
        {
            var t = o.GetType().GetProperty(p);
            if (t != null)
            {
                res = t.GetValue(o);
            }
        }

        return res;
    }

    /// <summary>
    /// Stripping out non-numeric characters in string
    /// </summary>
    /// <param name="o">Object need to check</param>
    /// <returns>Return the numeric string</returns>
    public static string ToNumber(this object o)
    {
        return Regex.Replace(o + "", "[^0-9]", "");
    }

    /// <summary>
    /// Convert the object to string with format
    /// </summary>
    /// <param name="o">Object</param>
    /// <param name="f">Format type</param>
    /// <returns>Return the string</returns>
    public static string ToStr(this object o, TextFormat f = TextFormat.Original)
    {
        var res = o == null ? string.Empty : o.ToString() + string.Empty;

        switch (f)
        {
            case TextFormat.Sentence:
                return res.ToUpperFirst();

            case TextFormat.Lower:
                return res.ToLower();

            case TextFormat.Upper:
                return res.ToUpper();

            case TextFormat.Capitalized:
                return res.ToUpperWords();

            default:
                return res;
        }
    }

    /// <summary>
    /// Convert the object to dictionary
    /// </summary>
    /// <param name="o">Object</param>
    /// <param name="formatKey">Format key</param>
    /// <returns>Return the result</returns>
    public static Dictionary<string, string> ToDictionary(this object o, string? formatKey = null)
    {
        Dictionary<string, string> res = [];
        var properties = o.GetType().GetProperties();

        if (string.IsNullOrWhiteSpace(formatKey) || !formatKey.Contains("{0}"))
        {
            formatKey += "{0}";
        }

        foreach (PropertyInfo i in properties)
        {
            var key = string.Format(formatKey, i.Name);
            res.Add(key, i.GetValue(o) + "");
        }

        return res;
    }

    #endregion
}

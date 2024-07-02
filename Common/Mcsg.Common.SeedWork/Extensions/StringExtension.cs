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

using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Mcsg.Common.SeedWork.Extensions;

using static Dtos.ConnectionDto;

/// <summary>
/// String extension for using [this string] only
/// </summary>
public static class StringExtension
{
    #region -- Methods --

    /// <summary>
    /// Set placeholder
    /// </summary>
    /// <param name="body">Text containing placeholders</param>
    /// <param name="dictionary">Dictionary placeholder</param>
    /// <returns>Return the result</returns>
    public static string SetPlaceholder(this string? body, Dictionary<string, string> dictionary)
    {
        var res = string.Empty;
        if (body == null || dictionary == null)
        {
            return res;
        }

        res = dictionary.Aggregate(body, (current, value) => current.Replace(value.Key, value.Value));
        return res;
    }

    /// <summary>
    /// Convert text to PascalCase https://stackoverflow.com/questions/18627112/how-can-i-convert-text-to-pascal-case
    /// </summary>
    /// <param name="text">Original string</param>
    /// <returns>Return the PascalCase text</returns>
    public static string ToPascalCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
        var whiteSpace = new Regex(@"(?<=\s)");
        var startsWithLowerCaseChar = new Regex("^[a-z]");
        var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
        var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
        var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

        // Replace white spaces with undescore, then replace all invalid chars with empty string
        var res = invalidCharsRgx.Replace(whiteSpace.Replace(text, "_"), string.Empty)
            // split by underscores
            .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
            // set first letter to uppercase
            .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
            // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
            .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
            // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
            .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
            // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
            .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

        return string.Concat(res);
    }

    /// <summary>
    /// Convert the text to camelCase
    /// </summary>
    /// <param name="text">Original string</param>
    /// <returns>Return the camelCase text</returns>
    public static string ToCamelCase(this string text)
    {
        var res = text.ToPascalCase();

        // First word
        var arr = res.ToCharArray();
        if (arr.Length >= 1)
        {
            arr[0] = char.ToLower(arr[0]);
        }

        res = new string(arr);

        return res;
    }

    /// <summary>
    /// Return a copy of this string with first letter converted to uppercase
    /// </summary>
    /// <param name="s">String data</param>
    /// <returns>Return uppercase first letter</returns>
    public static string ToUpperFirst(this string s)
    {
        var res = string.Empty;

        if (string.IsNullOrWhiteSpace(s))
        {
            return res;
        }

        s = s.Trim();
        res = char.ToUpper(s[0]) + s.Substring(1).ToLower();

        return res;
    }

    /// <summary>
    /// Return a copy of this string with first letter of each word converted to uppercase
    /// </summary>
    /// <param name="s">String data</param>
    /// <returns>Return uppercase first letter of any words</returns>
    public static string ToUpperWords(this string s)
    {
        var res = string.Empty;

        if (string.IsNullOrWhiteSpace(s))
        {
            return res;
        }

        s = s.Trim().ToLower();
        var arr = s.ToCharArray();

        // First word
        if (arr.Length >= 1)
        {
            arr[0] = char.ToUpper(arr[0]);
        }

        // Next word after space
        for (var i = 1; i < arr.Length; i++)
        {
            if (arr[i - 1] == ' ')
            {
                arr[i] = char.ToUpper(arr[i]);
            }
        }
        res = new string(arr);

        return res;
    }

    /// <summary>
    /// Get first character of each word
    /// </summary>
    /// <param name="s">String data</param>
    /// <returns>Return the string</returns>
    public static string ToInitial(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return string.Empty;
        }

        return string.Concat(s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(p => p[0]));
    }

    /// <summary>
    /// Convert string to list with separator and distinct
    /// </summary>
    /// <param name="s">String data</param>
    /// <param name="c">Separator (default is semicolon)</param>
    /// <param name="d">Distinct</param>
    /// <returns>Return the result</returns>
    public static List<string> ToSet(this string? s, char c = ';', bool d = true)
    {
        var res = new List<string>();

        if (string.IsNullOrWhiteSpace(s))
        {
            s = string.Empty;
        }

        var arr = s.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
        var t = arr.Where(p => !string.IsNullOrWhiteSpace(p));

        if (d)
        {
            res = t.Select(p => p.Trim()).Distinct().ToList();
        }
        else
        {
            res = t.Select(p => p.Trim()).ToList();
        }

        return res;
    }

    /// <summary>
    /// Convert a string value to enum value
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">Description or value need to convert</param>
    /// <param name="default">Default value</param>
    /// <returns>Return the enum value</returns>
    public static T? ToEnum<T>(this string? value, T @default)
    {
        var res = @default;

        if (!typeof(T).IsEnum)
        {
            return res;
        }

        var type = typeof(T);
        var l = type.GetFields();

        foreach (var i in l)
        {
            if (Attribute.GetCustomAttribute(i, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (i.Name == value)
                {
                    res = (T?)i.GetValue(null);
                    break;
                }

                if (attribute.Description == value)
                {
                    res = (T?)i.GetValue(null);
                    break;
                }
            }
            else
            {
                if (i.Name == value)
                {
                    res = (T?)i.GetValue(null);
                    break;
                }
            }
        }

        return res;
    }

    /// <summary>
    /// Get first name
    /// </summary>
    /// <param name="fullName">Full name</param>
    /// <param name="lastName">Last name</param>
    /// <returns>Return the result</returns>
    public static string ToFirstName(this string fullName, out string lastName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            fullName = string.Empty;
        }

        var res = fullName;
        var index = fullName.IndexOf(" ");
        if (index < 0)
        {
            lastName = string.Empty;
            return res;
        }

        lastName = fullName.Substring(index + 1).Trim();

        res = fullName.Substring(0, index).Trim();
        return res;
    }

    /// <summary>
    /// Get hashtags
    /// </summary>
    /// <param name="s">String data</param>
    /// <returns>Return the result</returns>
    public static List<string> ToHashtags(this string? s)
    {
        List<string> res = [];

        if (string.IsNullOrWhiteSpace(s))
        {
            return res;
        }

        Regex regex = new(@"#\w+");
        var matches = regex.Matches(s);
        foreach (Match match in matches)
        {
            res.Add(match.Value);
        }

        return res;
    }

    /// <summary>
    /// Converts the specified noun to its plural form
    /// </summary>
    /// <param name="s">The singular noun</param>
    /// <returns>The plural form of the noun</returns>
    public static string ToPlural(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return string.Empty;
        }

        string suffix;

        var isLower = s.IsLowerLastCharacter();
        if (isLower)
        {
            suffix = "s";

            if (s.EndsWith("s") || s.EndsWith("x") || s.EndsWith("z") || s.EndsWith("ch") || s.EndsWith("sh"))
            {
                suffix = "es";
            }

        }
        else
        {
            suffix = "S";

            if (s.EndsWith("S") || s.EndsWith("X") || s.EndsWith("Z") || s.EndsWith("CH") || s.EndsWith("SH"))
            {
                suffix = "ES";
            }
        }

        return s + suffix;
    }

    /// <summary>
    /// Checks if the last character of the given word is uppercase.
    /// </summary>
    /// <param name="word">The word to check.</param>
    /// <returns>True if the last character is uppercase, otherwise false.</returns>
    public static bool IsUpperLastCharacter(this string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return false;
        }

        var t = word[word.Length - 1];
        return char.IsUpper(t);
    }

    /// <summary>
    /// Checks if the last character of the given word is lowercase.
    /// </summary>
    /// <param name="word">The word to check.</param>
    /// <returns>True if the last character is lowercase, otherwise false.</returns>
    public static bool IsLowerLastCharacter(this string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return false;
        }

        var t = word[word.Length - 1];
        return char.IsLower(t);
    }

    /// <summary>
    /// Be less than or equal max bytes
    /// </summary>
    /// <param name="s">String data</param>
    /// <param name="max">Maximum length</param>
    /// <returns>Return the result</returns>
    public static bool BeLessThanOrEqualMaxBytes(this string? s, ushort max)
    {
        if (string.IsNullOrEmpty(s))
        {
            return true;
        }

        var byteCount = Encoding.UTF8.GetByteCount(s);

        return byteCount <= max;
    }

    /// <summary>
    /// Set database parameters {DbServer} {DbPort} {DbName} {DbUser} {DbPassword}
    /// </summary>
    /// <param name="cs">Connection string</param>
    /// <param name="db">Database setting</param>
    /// <returns>Return the connection string</returns>
    public static string SetDbParams(this string? cs, DatabaseDto db)
    {
        ArgumentNullException.ThrowIfNull(db, nameof(db));

        var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "{DbServer}", db.Host },
            { "{DbPort}", db.Port.ToString() },
            { "{DbName}", db.Name },
            { "{DbUser}", db.UserName },
            { "{DbPassword}", db.Password }
        };

        return cs.SetPlaceholder(dic);
    }

    /// <summary>
    /// Mask digits
    /// </summary>
    /// <param name="s">String data</param>
    /// <param name="first">First</param>
    /// <param name="last">Last</param>
    /// <returns>Return the result</returns>
    public static string MaskDigits(this string s, int first, int last)
    {
        // Take first 6 characters
        var firstPart = s.Substring(0, first);

        // Take last 4 characters
        int len = s.Length;
        string lastPart = s.Substring(len - last, last);

        // Take the middle part (****)
        int middlePartLenght = len - (firstPart.Length + lastPart.Length);
        string middlePart = new String('*', middlePartLenght);

        return firstPart + middlePart + lastPart;
    }

    /// <summary>
    /// Trims the whitespace from both ends of the string.  Whitespace is defined by char.IsWhiteSpace
    /// </summary>
    /// <param name="s">String data</param>
    /// <returns>Return the result</returns>
    public static string Triz(this string? s)
    {
        return (s + "").Trim();
    }
    #endregion
}

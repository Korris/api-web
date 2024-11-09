using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Mcsg.Common.Extensions;

public static class StringExtensions
{
    public static Guid ToGuid(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Guid.Empty;
            //throw new ArgumentException("Input string cannot be null or empty.", nameof(input));
        }
        if (Guid.TryParse(input, out Guid result))
        {
            return result;
        }
        throw new FormatException("Input string is not in a valid Guid format.");
    }
    public static string ToPg(this string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? string.Format("\"{0}\"", input) : string.Empty;
    }
    public static bool IsNumeric(this string text)
    {
        double _out;
        return double.TryParse(text, out _out);
    }
    public static int ToKiloNumber(this string str)
    {
        int number = 0;
        int abbreviation = 1;
        str = str.ToUpper();
        if (str.Contains("K"))
        {
            abbreviation = 1000;
        }
        else if (str.Contains("M"))
        {
            abbreviation = 1000000;
        }
        else if (str.Contains("B"))
        {
            abbreviation = 1000000000;
        }

        str = str.Replace(".", "").Replace(",", "").Replace("K", "").Replace("M", "").Replace("B", "").Trim();

        Int32.TryParse(str, out number);
        number = number * abbreviation;

        return number;
    }
    public static string ToCapitalize(this string str)
    {
        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(str);
    }
    public static Int32 ToInt32(this string number)
    {
        if (string.IsNullOrWhiteSpace(number)) { return 0; }
        if (!IsNumeric(number)) { return 0; }
        return Int32.Parse(number, NumberStyles.Integer, CultureInfo.CurrentCulture.NumberFormat);
    }
    public static Int64 ToInt64(this string number)
    {
        if (string.IsNullOrWhiteSpace(number)) { return 0; }
        if (!IsNumeric(number)) { return 0; }
        return Int64.Parse(number, NumberStyles.Integer, CultureInfo.CurrentCulture.NumberFormat);
    }
    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
    public static string ToHashtags(this string str)
    {
        // Remove diacritics from the text
        var text = str.RemoveDiacritics();

        // Remove Vietnamese from the text
        text = text.RemoveSign4VietnameseString();

        // Convert text to lowercase
        text = text.ToLower();

        // Remove non-word characters except for whitespace
        text = Regex.Replace(text, @"[^\w\s]", "");

        // Remove whitespace
        text = text.Replace(" ", "");

        return text;
    }
    public static string RemoveSign4VietnameseString(this string str)
    {
        for (int i = 1; i < VietnameseSigns.Length; i++)
        {
            for (int j = 0; j < VietnameseSigns[i].Length; j++)
                str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
        }
        return str;
    }

    private static readonly string[] VietnameseSigns = new string[]
    {

        "aAeEoOuUiIdDyY",

        "áàạảãâấầậẩẫăắằặẳẵ",

        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

        "éèẹẻẽêếềệểễ",

        "ÉÈẸẺẼÊẾỀỆỂỄ",

        "óòọỏõôốồộổỗơớờợởỡ",

        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

        "úùụủũưứừựửữ",

        "ÚÙỤỦŨƯỨỪỰỬỮ",

        "íìịỉĩ",

        "ÍÌỊỈĨ",

        "đ",

        "Đ",

        "ýỳỵỷỹ",

        "ÝỲỴỶỸ"
    };
}

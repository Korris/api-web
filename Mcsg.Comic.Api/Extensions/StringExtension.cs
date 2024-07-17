namespace Mcsg.Comic.Api.Extensions;

using Common.Core.Extensions;

public static class StringExtension
{
    public static int ToInt(this string text)
    {
        int _out;
        var isNumeric = Int32.TryParse(text, out _out);
        return _out;
    }

    public static string ToPublicImageUrl(this string fileName)
    {
        return Program._mediaApiUrl.ToPublicImageUrl(fileName);
    }
}

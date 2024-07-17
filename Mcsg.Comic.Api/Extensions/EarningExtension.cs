namespace Mcsg.Comic.Api.Extensions;

using Common.Core.Enums;

public static class EarningExtension
{
    public static string ToDisplay(this PostType value)
    {
        var enumDisplayStatus = (PostType)value;
        return enumDisplayStatus.ToString();
    }
    public static string ToDisplay(this PostStatus value)
    {
        var enumDisplayStatus = (PostStatus)value;
        return enumDisplayStatus.ToString();
    }
}

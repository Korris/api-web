namespace Mcsg.Lib.Common.Extensions;

public static class EnumExtensions
{
    public static T ToEnum<T>(this string text)
    {
        return (T)Enum.Parse(typeof(T), text);
    }
}

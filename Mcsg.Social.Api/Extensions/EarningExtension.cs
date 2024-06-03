using Mcsg.Lib.Model.Enums;

namespace Mcsg.Api.Extensions
{
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
}

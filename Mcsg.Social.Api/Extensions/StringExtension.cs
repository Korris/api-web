namespace Mcsg.Social.Api.Extensions
{
    public static class StringExtension
    {
        public static int ToInt(this string text)
        {
            int _out;
            var isNumeric = Int32.TryParse(text, out _out);
            return _out;
        }
    }
}

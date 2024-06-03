using System.Text;

namespace Mcsg.Function.Job.Helpers
{
    public static class StringHelper
    {
        public static string GetRandomString(int length)
        {
            string text = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                char value = text[random.Next(0, text.Length)];
                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }
        public static string MaskDigits(string input, int first, int last)
        {
            //take first 6 characters
            string firstPart = input.Substring(0, first);

            //take last 4 characters
            int len = input.Length;
            string lastPart = input.Substring(len - last, last);

            //take the middle part (****)
            int middlePartLenght = len - (firstPart.Length + lastPart.Length);
            string middlePart = new String('*', middlePartLenght);

            return firstPart + middlePart + lastPart;
        }
    }
}

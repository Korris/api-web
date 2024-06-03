using System.Text;

namespace Mcsg.Lib.Common.Helpers
{
    public static class StringGenerator
    {
        public static string GetRandomString(int length)
        {
            string letters = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder();
            Random random = new Random();
            for (var i = 0; i < length; i++)
            {
                var c = letters[random.Next(0, letters.Length)];
                builder.Append(c);
            }
            return builder.ToString();
        }

        public static string GenerateOtp(int length = 6)
        {
            // Define characters that can be used in the OTP (typically digits)
            const string chars = "0123456789";

            // Create a random number generator
            var random = new Random();

            // Generate a random OTP of the specified length
            var otp = new char[length];
            for (var i = 0; i < length; i++)
            {
                otp[i] = chars[random.Next(chars.Length)];
            }

            return new string(otp);
        }
    }
}

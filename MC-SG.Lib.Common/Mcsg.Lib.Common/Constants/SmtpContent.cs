namespace Mcsg.Lib.Common.Constants
{
    public static class SmtpContent
    {
        public const string VerifyOtpEmailTitle = "Verify your email - OTP CODE ";
        public const string EmailConfirmUrl = "/VerifyMailToken/";
        public const string VerifyOtpEmailBody = "OTP code is {0} \n Will expired in 2 minutes";
        public const string ResetPassByEmailTitle = "Request reset password - OTP CODE";
        public const string ResetPassByEmailBody = "You request reset password. Here your OTP code is {0} \n Will expired in 2 minutes";
    }
}

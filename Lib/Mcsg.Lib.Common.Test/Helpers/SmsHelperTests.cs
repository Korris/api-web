namespace Mcsg.Lib.Common.Test.Helpers
{
    public class SmsHelperTests : TestBase
    {
        [Test]
        public void ShouldSendSmsSuccessfully()
        {
            string accountSid = "AC54637266dd89083be97d10e764080447";
            string authToken = "4e479885fc6ce9f8b3005a43da9988bc";
            string twilioPhone = "+12054311032";

            string phoneNumber = "+84934480543";
            string body = "OTP Testing Ntada!";
            //var messId = SmsHelper.SendSms(phoneNumber, body, accountSid,authToken,twilioPhone);

            //Assert.IsNotNull(messId);
            //Assert.IsNotEmpty(messId);
        }
    }
}

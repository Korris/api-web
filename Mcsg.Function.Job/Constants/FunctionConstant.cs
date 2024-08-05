namespace Mcsg.Function.Job.Constants;

internal class FunctionConstant
{
    public const string StorageName = "Function:StorageName";
    public const string AccountKey = "Function:AccountKey";

    public const string DbConnectionString = "Function:DbConnectString";
    public const string WalletDbConnectionString = "Function:WalletDbConnectString";
    public const string PublicStorageConnection = "Function:PublicStorageConnection";

    public const string EmailHost = "Function:Email:SmtpHost";
    public const string EmailUser = "Function:Email:SmtpUser";
    public const string EmailPass = "Function:Email:SmtpPass";
    public const string EmailFrom = "Function:Email:SmtpFrom";
    public const string EmailPort = "Function:Email:SmtpPort";
    public const string EmailDisplayFrom = "Function:Email:DisplayFrom";

    public const string LogoUrl = "https://hcm03.vstorage.vngcloud.vn/v1/AUTH_9b7a1d8050f74dacb2c6f920b9c25853/bumcheo-pro-public/images/logo.png";

    public const string OtpEmailTitle = "Verify your email - OTP CODE";
    public const string WithdrawEmailTitle = "Withdraw request";
    public const string DepositEmailTitle = "Deposit request";

    public const string TwilioAccountSid = "Function:Twilio:AccountSid";
    public const string TwilioAuthToken = "Function:Twilio:AuthToken";
    public const string TwilioPhoneNumber = "Function:Twilio:PhoneNumber";
    public const string ExclusiveUnlockCron = "%Function:ExclusiveUnlockCron%";

    public const string ZaloPayAppId = "Function:ZaloPay:AppId";
    public const string ZaloPayAppUser = "Function:ZaloPay:AppUser";
    public const string ZaloPayKey1 = "Function:ZaloPay:Key1";
    public const string ZaloPayKey2 = "Function:ZaloPay:Key2";
    public const string ZaloPayUrl = "Function:ZaloPay:Url";

    public const string RealTimeServiceUrl = "Function:RealTimeService:BaseUrl";
}

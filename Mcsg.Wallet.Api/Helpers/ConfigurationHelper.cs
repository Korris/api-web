namespace Mcsg.Wallet.Api;

public static class ConfigurationHelper
{
    public static int OtpExpired(this IConfiguration configuration)
    {
        return int.Parse(configuration["OtpSetting:OtpExpired"]);
    }
}

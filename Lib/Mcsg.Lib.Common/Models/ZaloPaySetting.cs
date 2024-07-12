namespace Mcsg.Lib.Common.Models;

public class ZaloPaySetting
{
    public static string ConfigName => "ZaloPay";
    public string AppUser { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public string CallBackUrl { get; set; } = string.Empty;
    public int AppId { get; set; }
    public string Key1 { get; set; } = string.Empty;
    public string Key2 { get; set; } = string.Empty;
    public int QueryScheduleMinutes { get; set; }
}

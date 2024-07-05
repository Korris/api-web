namespace Mcsg.Admin.Api.Dtos
{
    public class EmailSettingRespone : BaseSystemSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}

namespace Mcsg.Admin.Api.DTOs.Settings
{
    public class EmailSettingRequest
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}

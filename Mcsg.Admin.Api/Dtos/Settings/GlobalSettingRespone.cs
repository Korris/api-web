namespace Mcsg.Admin.Api.Dtos
{
    public class GlobalSettingRespone : BaseSystemSetting
    {
        public string Favicon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

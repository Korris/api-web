namespace Mcsg.Admin.Api.Requests
{
    public class GlobalSettingRequest
    {
        public IFormFile Favicon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

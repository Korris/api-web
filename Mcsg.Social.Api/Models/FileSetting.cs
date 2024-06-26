namespace Mcsg.Social.Api.Models
{
    public class FileSetting
    {
        public int UploadMultipartBodyLengthLimit { get; set; }
        public int UploadValueLengthLimit { get; set; }
        public string MediaExtensionAllow { get; set; }
        public int ImageDownQuality { get; set; }
    }
}

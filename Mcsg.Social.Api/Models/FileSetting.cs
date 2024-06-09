namespace Mcsg.Social.Api.Models
{
    public class FileSetting
    {
        public int UploadMultipartBodyLengthLimit { get; set; }
        public int UploadValueLengthLimit { get; set; }
        public string MediaExtensionAllow { get; set; }
        public string MediaEncryptKey { get; set; }
        public string MediaUrl { get; set; }
        public string MediaCDNUrl { get; set; }
        public int ImageDownQuality { get; set; }
    }
}

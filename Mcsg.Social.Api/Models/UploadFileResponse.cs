using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Models
{
    public class UploadFileResponse
    {
        public string HashId { get; set; }
        public string Url { get; set; }
        public string ShareUrl { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public ResourceStatus Status { get; set; }
        public ResourceType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Size { get; set; }
    }
    public class UploadFileQueryDbResponse : UploadFileResponse
    {
        public Guid Id { get; set; }
    }
}

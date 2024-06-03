using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Models
{
    public class ResourceResponse
    {
        public ResourceType Type { get; set; }
        public ResourceStatus Status { get; set; }
        public int Order { get; set; }
        public string Url { get; set; }
        public string ShareUrl { get; set; }
        public string Name { get; set; }
        public string HashId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

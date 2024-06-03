using Microsoft.AspNetCore.Http;

namespace Mcsg.Lib.Common.Models
{
    public class CompressImage : ImageRatio
    {
        public IFormFile Image { get; set; }
    }
}

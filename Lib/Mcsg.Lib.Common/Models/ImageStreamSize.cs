namespace Mcsg.Lib.Common.Models;

using Mcsg.Common.Core.Dtos;

public class ImageStreamSize : ImageRatio
{
    public Stream Stream { get; set; }
    public long Length { get; set; }
}

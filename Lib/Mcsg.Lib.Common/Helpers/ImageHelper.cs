using System.Drawing;
using System.Drawing.Drawing2D;
using Image = System.Drawing.Image;

namespace Mcsg.Lib.Common.Helpers
{
    public static class ImageHelper
    {
        public static MemoryStream ResizeImage(Stream imageContent, int width, int height)
        {
            using (var image = Image.FromStream(imageContent))
            {
                using (var resizedImage = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, width, height);
                    }

                    var memoryStream = new MemoryStream();
                    resizedImage.Save(memoryStream, image.RawFormat);

                    memoryStream.Position = 0;

                    return memoryStream;
                }
            }
        }

    }
}

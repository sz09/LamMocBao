using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Services.Utiltities
{
    public class ImageHelper
    {
        public static MemoryStream GetSmallImageBytes(Image image)
        {
            Size squareSize = new Size(image.Width, image.Height);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = Graphics.FromImage(squareImage))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, squareSize.Width, squareSize.Height);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.DrawImage(image, (squareSize.Width / 2) - (image.Width / 2), (squareSize.Height / 2) - (image.Height / 2), image.Width, image.Height);
            }
            var ratio = image.Width / 50;
            var newSmallImage = new Bitmap(50, squareSize.Height / ratio);
            Graphics.FromImage(newSmallImage).DrawImage(squareImage, 0, 0, 50, squareSize.Height / ratio);
            Bitmap bmpSmall = new Bitmap(newSmallImage);

            byte[] byteArraySmall = new byte[0];

            using (MemoryStream streamSmall = new MemoryStream())
            {
                bmpSmall.Save(streamSmall, ImageFormat.Png);
                byteArraySmall = streamSmall.ToArray();
            }
            return new MemoryStream(byteArraySmall);
        }
    }
}

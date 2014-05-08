using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ObjectDetect
{
    internal static class ImageProcessing
    {
        internal static Bitmap<int> IntegralImage(this string fileName)
        {
            using (Bitmap bitmap = new Bitmap(fileName), grayScale = bitmap.Grayscale())
            {
                return grayScale.IntegralImage();
            }
        }

        internal static Bitmap<int> IntegralImage(this Bitmap image)
        {
            var imData = image.LockBits(
                new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                image.PixelFormat);

            var width = image.Width;
            var height = image.Height;
            var dst = new int[height][];

            //sum each row independently
            Parallel.For(0, height, y => dst[y] = imData.SumRow(y * imData.Stride, width));

            image.UnlockBits(imData);

            //sum columns
            for (var y = 1; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var prevSum = dst[y - 1][x];
                    var thisSum = dst[y][x];
                    dst[y][x] = prevSum + thisSum;
                }
            }

            var integralImage = new Bitmap<int>(dst, width, height);
            return integralImage;
        }

        internal static Bitmap Grayscale(this Image src)
        {
            var image = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
            try
            {
                var theWholeImage = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                using (var gr = Graphics.FromImage(image))
                {
                    using (var attr = new ImageAttributes())
                    {
                        attr.SetColorMatrix(new ColorMatrix(
                            new[]
                            {
                                new[] {.3f, .3f, .3f, 0, 0},
                                new[] {.59f, .59f, .59f, 0, 0},
                                new[] {.11f, .11f, .11f, 0, 0},
                                new[] {0, 0, 0, 1f, 0},
                                new[] {0, 0, 0, 0, 1f}
                            }));
                        gr.DrawImage(src, theWholeImage, 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, attr);
                    }
                }
            }
            catch (Exception)
            {
                image.Dispose();
                throw;
            }
            return image;
        }

        private static unsafe int[] SumRow(this BitmapData src, int offset, int width)
        {
            if (src.PixelFormat != PixelFormat.Format24bppRgb) throw new ArgumentException("Incorrect pixel format", "src");
            const int pixSize = 3;

            var srcRow = (byte*)src.Scan0 + offset;
            var dstRow = new int[width];
            var accumulator = 0;

            for (var x = 0; x < width; x++)
            {
                accumulator += srcRow[x * pixSize];
                dstRow[x] = accumulator;
            }
            return dstRow;
        }
    }
}

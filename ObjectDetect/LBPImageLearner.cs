using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using AdaBoost;

namespace ObjectDetect
{
    class LBPImageLearner : ILearner<ImageSample>
    {
        private ImageSample _sample;
        private readonly Configuration _config;
        private Bitmap<int> _integralImage;

        private LBPImageLearner(Configuration config, LBPImageLearner lbpImageLearner)
        {
            _config = config;
            _sample = lbpImageLearner._sample;
            _integralImage = lbpImageLearner._integralImage;
        }

        public LBPImageLearner()
        {
            _config = Configuration.Parse(AllPossibleConfigurations().First());
            _sample = null;
            _integralImage = null;
        }

        private struct Configuration
        {
            internal readonly int RelativeScale;
            internal readonly int Bucket;

            internal const int NumSurroundingPixels = 8;

            internal const int NumRelativeScaleLevels = 5;
            internal const int ScaleChangePerLevel = 8;

            //clockwise, starting from top left
            internal static readonly int[] XOffsets = { -1, 0, 1, 1, 1, 0, -1, -1 };
            internal static readonly int[] YOffsets = { -1, -1, -1, 0, 1, 1, 1, 0 };

            internal Configuration(int scale, int bucket)
            {
                RelativeScale = scale;
                Bucket = bucket;
            }

            internal static Configuration Parse(string configString)
            {
                var c = configString.Split(',');
                byte scale, bucket;
                if (c.Length != 2 || !byte.TryParse(c[0], out scale) || !byte.TryParse(c[1], out bucket))
                {
                    throw new ArgumentException();
                }
                return new Configuration(scale, bucket);
            }

            public override string ToString()
            {
                return RelativeScale + "," + Bucket;
            }
        }

        public string Config { get { return _config.ToString(); } }

        public const string UniqueIdString = "LBPImageLearner";

        public float Classify()
        {
            var zoom = _sample.GetZoomLevelAtScale(_sample.Scale + _config.RelativeScale);
            //int[] buckets = new int[(1 << Configuration.numSurroundingPixels) - 1];
            var retVal = 0;
            for (var x = 0; x < _sample.PixelCount; x++) {
                for (var y = 0; y < _sample.PixelCount; y++)
                {
                    var center = GetPixelValue(x, y, zoom, _sample.Location.X, _sample.Location.Y);
                    var bucket = 0;
                    for (var i = 0; i < Configuration.NumSurroundingPixels; i++)
                    {
                        bucket <<= 1;
                        if (GetSurroundingPixVal(i, x, y, zoom, _sample.Location.X, _sample.Location.Y) > center)
                        {
                            bucket |= 1;
                        }
                        if (bucket == _config.Bucket) retVal++;
                        //buckets[bucket]++;
                    }
                }
            }
            return retVal;
            //return buckets[config.bucket];
        }

        private FixedPoint GetSurroundingPixVal(int n, int x, int y, FixedPoint zoom, FixedPoint xBase, FixedPoint yBase)
        {
            return GetPixelValue(x + Configuration.XOffsets[n], y + Configuration.YOffsets[n], zoom, xBase, yBase);
        }

        private FixedPoint GetPixelValue(int x, int y, FixedPoint zoom, FixedPoint xBase, FixedPoint yBase)
        {
            var xloc = xBase + x * zoom - 1;
            var yloc = yBase + y * zoom - 1;
            var topLeft = Interpolate(xloc, yloc);
            var lowLeft = Interpolate(xloc, yloc + zoom);
            var topRight = Interpolate(xloc + zoom, yloc);
            var lowRight = Interpolate(xloc + zoom, yloc + zoom);
            return lowRight - topRight - lowLeft + topLeft;
        }

        private FixedPoint Interpolate(FixedPoint x, FixedPoint y)
        {
            var wholeX = x.Floor();
            var wholeY = y.Floor();
            var fracX = x - wholeX;
            var fracY = y - wholeY;

            /*           Key:
             * +++++^    +: sum
             * +++++^    ^: rt
             * +++++^    ~: lo
             * ~~~~~&    &: lr
             * 
             */

            var sum = InImage(wholeX, wholeY) ? _integralImage[wholeX, wholeY] : 0;
            var rt = InImage(wholeX, wholeY + 1) ? _integralImage[wholeX, wholeY + 1] - sum : 0;
            var lo = InImage(wholeX + 1, wholeY) ? _integralImage[wholeX + 1, wholeY] - sum : 0;
            var lr = InImage(wholeX + 1, wholeY + 1) ? _integralImage[wholeX + 1, wholeY + 1] - (sum + rt + lo) : 0;

            return sum + rt * fracX + lo * fracY + lr * fracX * fracY;
        }

        private bool InImage(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _integralImage.Width && y < _integralImage.Height;
        }

        private static readonly ConcurrentDictionary<string, WeakReference<Bitmap<int>>> IntImageCache = new ConcurrentDictionary<string, WeakReference<Bitmap<int>>>();

        public void SetSample(ImageSample value)
        {
            if (!value.FileEquals(_sample))
            {
                var entry = IntImageCache.GetOrAdd(value.FileName, new WeakReference<Bitmap<int>>(null));
                lock (entry)
                {
                    if (!entry.TryGetTarget(out _integralImage))
                    {
                        var src = new Bitmap(value.FileName);
                        var image = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
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
                        src.Dispose();

                        var imData = image.LockBits(
                            theWholeImage,
                            ImageLockMode.ReadOnly,
                            image.PixelFormat);

                        var width = image.Width;
                        var height = image.Height;
                        var dst = new int[height][];

                        //sum each row independently
                        Parallel.For(0, height, y => dst[y] = SumRow(y*imData.Stride, width, imData));

                        image.UnlockBits(imData);
                        image.Dispose();


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

                        _integralImage = new Bitmap<int>(dst, width, height);

                        entry.SetTarget(_integralImage);
                    }
                }
            }
            _sample = value;
        }

        private static unsafe int[] SumRow(int offset, int width, BitmapData src)
        {
            if (src.PixelFormat != PixelFormat.Format24bppRgb) throw new ArgumentException("src");
            const int pixSize = 3;

            var srcRow = (byte*) src.Scan0 + offset;
            var dstRow = new int[width];
            var accumulator = 0;

            for (var x = 0; x < width; x++)
            {
                accumulator += srcRow[x*pixSize];
                dstRow[x] = accumulator;
            }
            return dstRow;
        }

        public ILearner<ImageSample> WithConfiguration(string configuration)
        {
            return new LBPImageLearner(Configuration.Parse(configuration), this);
        }

        public string UniqueId
        {
            get { return UniqueIdString; }
        }

        public override string ToString()
        {
            return UniqueId + " [" + _config.ToString() + "]";
        }

        public IEnumerable<string> AllPossibleConfigurations()
        {
            for (var scaleLevel = 0; scaleLevel < Configuration.NumRelativeScaleLevels; scaleLevel++)
            {
                for (var bucket = 0; bucket < (1 << Configuration.NumSurroundingPixels); bucket++)
                {
                    yield return new Configuration(scaleLevel * Configuration.ScaleChangePerLevel, bucket).ToString();
                }
            }
        }
    }
}

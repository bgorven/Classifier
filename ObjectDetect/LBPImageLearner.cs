using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ObjectDetect
{
    class LBPImageLearner : AdaBoost.ILearner<ImageSample>
    {
        private ImageSample _sample;
        private readonly Configuration _config;
        private Bitmap<int> _integralImage;

        private LBPImageLearner(Configuration config, LBPImageLearner lbpImageLearner)
        {
            this._config = config;
            this._sample = lbpImageLearner._sample;
            this._integralImage = lbpImageLearner._integralImage;
        }

        public LBPImageLearner()
        {
            this._config = Configuration.Parse(GetPossibleParams().First());
            this._sample = null;
            this._integralImage = null;
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
                this.RelativeScale = scale;
                this.Bucket = bucket;
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

        public float Classify()
        {
            FixedPoint zoom = _sample.GetZoomLevelAtScale(_sample.Scale + _config.RelativeScale);
            //int[] buckets = new int[(1 << Configuration.numSurroundingPixels) - 1];
            int retVal = 0;
            for (int x = 0; x < _sample.PixelCount; x++) {
                for (int y = 0; y < _sample.PixelCount; y++)
                {
                    FixedPoint center = GetPixelValue(x, y, zoom, _sample.Location.X, _sample.Location.Y);
                    int bucket = 0;
                    for (int i = 0; i < Configuration.NumSurroundingPixels; i++)
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
            return GetPixelValue(x + Configuration.XOffsets[n], y + Configuration.YOffsets[n], zoom, _sample.Location.X, _sample.Location.Y);
        }

        private FixedPoint GetPixelValue(int x, int y, FixedPoint zoom, FixedPoint xBase, FixedPoint yBase)
        {
            FixedPoint xloc = xBase + x * zoom - 1;
            FixedPoint yloc = xBase + x * zoom - 1;
            FixedPoint topLeft = Interpolate(xloc, yloc);
            FixedPoint lowLeft = Interpolate(xloc, yloc + zoom);
            FixedPoint topRight = Interpolate(xloc + zoom, yloc);
            FixedPoint lowRight = Interpolate(xloc + zoom, yloc + zoom);
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

        public void SetSample(ImageSample s)
        {
            if (!this._sample.FileEquals(s))
            {
                var entry = IntImageCache.GetOrAdd(s.FileName, new WeakReference<Bitmap<int>>(null));
                lock (entry)
                {
                    if (!entry.TryGetTarget(out _integralImage))
                    {
                        var image = new System.Drawing.Bitmap(s.FileName);

                        //TODO
                        //
                        //var srcData = image.LockBits(
                        //    new System.Drawing.Rectangle(0, 0, image.Width, image.Height), 
                        //    System.Drawing.Imaging.ImageLockMode.ReadOnly, 
                        //    System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

                        //var src = new short[srcData.Stride * srcData.Height];
                        //var stride = srcData.Stride;

                        //Endianness?!
                        //System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, src, 0, src.Length);
                        //image.UnlockBits(srcData);

                        var stride = image.Width;
                        var dst = new int[image.Width * image.Height];
                        
                        
                        
                        //sum each row independently
                        Parallel.For(0, image.Height, y =>
                        {
                            int accumulator = 0;
                            int rowBase = y * stride;

                            for (int x = 0; x < image.Width; x++)
                            {
                                accumulator += (int)Math.Round(image.GetPixel(x, y).GetBrightness() * 256);
                                dst[rowBase + x] = accumulator;
                            }
                        });
                        //sum columns
                        for (int y = 1; y < image.Height; y++)
                        {
                            int rowBase = y * stride;
                            int prevRow = (y - 1) * stride;

                            for (int x = 0; x < image.Width; x++)
                            {
                                var prevSum = dst[prevRow + x];
                                var thisSum = dst[rowBase + x];
                                dst[rowBase + x] = prevSum + thisSum;
                            }
                        }

                        _integralImage = new Bitmap<int>(dst, image.Width, image.Height, stride);

                        entry.SetTarget(_integralImage);
                    }
                }
            }
            this._sample = s;
        }

        public AdaBoost.ILearner<ImageSample> WithParams(string parameters)
        {
            return new LBPImageLearner(Configuration.Parse(parameters), this);
        }

        public string GetUniqueIdString()
        {
            return "LBPImageSample";
        }

        public override string ToString()
        {
            return GetUniqueIdString() + " [" + _config.ToString() + "]";
        }

        public IEnumerable<string> GetPossibleParams()
        {
            for (int scaleLevel = 0; scaleLevel < Configuration.NumRelativeScaleLevels; scaleLevel++)
            {
                for (int bucket = 0; bucket < (1 << Configuration.NumSurroundingPixels); bucket++)
                {
                    yield return new Configuration(scaleLevel * Configuration.ScaleChangePerLevel, bucket).ToString();
                }
            }
        }
    }
}

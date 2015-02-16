using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows;
using AdaBoost;
using Utilities.Caching;

namespace ObjectDetect
{
    class LBPImageLearner : ILearner<ImageSample>
    {
        private ImageSample _sample;
        private readonly Configuration _config;
        private Bitmap<int> _integralImage;
        private int[] _histogram;
        private int _histogramScale;

        private LBPImageLearner(Configuration config, LBPImageLearner lbpImageLearner)
        {
            _config = config;
            if (lbpImageLearner._sample != null)
            {
                _sample = lbpImageLearner._sample;
                _integralImage = lbpImageLearner._integralImage;
                _histogramScale = _sample.Scale + config.RelativeScale;
                if (_histogramScale == lbpImageLearner._histogramScale)
                {
                    _histogram = lbpImageLearner._histogram;
                }
                else
                {
#if DEBUG
                    if (config.Bucket != 0)
                    {
                        throw new Exception();
                    }
#endif
                    _histogram = Histogram(_sample, _histogramScale);
                }
            }
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
                if (configString == null) throw new ArgumentNullException("configString");
                var c = configString.Split(',');
                byte scale, bucket;
                if (c.Length != 2 || !byte.TryParse(c[0], out scale) || !byte.TryParse(c[1], out bucket))
                {
                    throw new FormatException("configuration \"" + configString + "\" is not formatted correctly.");
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
            try
            {
                return _histogram[_config.Bucket];
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show(e.Message);
                return 0f;
            }
        }

        private int[] Histogram(ImageSample sample, int histogramScale)
        {
            var buckets = new int[1 << Configuration.NumSurroundingPixels];
            var zoom = sample.GetZoomLevelAtScale(histogramScale);

            for (var x = 0; x < sample.PixelCount; x++)
            {
                for (var y = 0; y < sample.PixelCount; y++)
                {
                    var center = GetPixelValue(x, y, zoom, sample.Location.X, sample.Location.Y);
                    var bucket = 0;
                    for (var i = 0; i < Configuration.NumSurroundingPixels; i++)
                    {
                        bucket <<= 1;
                        if (GetSurroundingPixVal(i, x, y, zoom, sample.Location.X, sample.Location.Y) > center)
                        {
                            bucket |= 1;
                        }
                        buckets[bucket]++;
                    }
                }
            }
            return buckets;
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

        private const int CacheSize = 128;
        private static readonly SimpleLRU<string, Bitmap<int>> IntImageCache = new SimpleLRU<string, Bitmap<int>>(CacheSize);

        public void SetSample(ImageSample sample)
        {
            if (sample == null)
            {
                throw new ArgumentNullException("sample");
            }
            if (sample != _sample)
            {
                if (!sample.FileEquals(_sample))
                {
                    _integralImage = IntImageCache.GetOrAdd(sample.FileName, sample.FileName.IntegralImage);
                }
                _sample = sample;
                _histogramScale = sample.Scale + _config.RelativeScale;
                _histogram = Histogram(sample, _histogramScale);
            }
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

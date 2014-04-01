using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ObjectDetect
{
    class LBPImageLearner : AdaBoost.ILearner<ImageSample>
    {
        private ImageSample sample;
        private readonly Configuration config;
        private Bitmap<int> integralImage;

        private LBPImageLearner(Configuration config, LBPImageLearner lBPImageLearner)
        {
            this.config = config;
            this.sample = lBPImageLearner.sample;
            this.integralImage = lBPImageLearner.integralImage;
        }

        public LBPImageLearner()
        {
            this.config = Configuration.Parse(getPossibleParams().First());
            this.sample = null;
            this.integralImage = null;
        }

        private struct Configuration
        {
            internal readonly int relativeScale;
            internal readonly int bucket;

            internal const int numSurroundingPixels = 8;

            internal const int numRelativeScaleLevels = 5;
            internal const int scaleChangePerLevel = 8;

            //clockwise, starting from top left
            internal static readonly int[] xOffsets = { -1, 0, 1, 1, 1, 0, -1, -1 };
            internal static readonly int[] yOffsets = { -1, -1, -1, 0, 1, 1, 1, 0 };

            internal Configuration(int scale, int bucket)
            {
                this.relativeScale = scale;
                this.bucket = bucket;
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
                return relativeScale + "," + bucket;
            }
        }

        public float classify()
        {
            fixed_point zoom = sample.getZoomLevelAtScale(sample.scale + config.relativeScale);
            //int[] buckets = new int[(1 << Configuration.numSurroundingPixels) - 1];
            int retVal = 0;
            for (int x = 0; x < sample.pixelCount; x++) {
                for (int y = 0; y < sample.pixelCount; y++)
                {
                    fixed_point center = getPixelValue(x, y, zoom, sample.location.x, sample.location.y);
                    int bucket = 0;
                    for (int i = 0; i < Configuration.numSurroundingPixels; i++)
                    {
                        bucket <<= 1;
                        if (getSurroundingPixVal(i, x, y, zoom, sample.location.x, sample.location.y) > center)
                        {
                            bucket |= 1;
                        }
                        if (bucket == config.bucket) retVal++;
                        //buckets[bucket]++;
                    }
                }
            }
            return retVal;
            //return buckets[config.bucket];
        }

        private fixed_point getSurroundingPixVal(int n, int x, int y, fixed_point zoom, fixed_point xBase, fixed_point yBase)
        {
            return getPixelValue(x + Configuration.xOffsets[n], y + Configuration.yOffsets[n], zoom, sample.location.x, sample.location.y);
        }

        private fixed_point getPixelValue(int x, int y, fixed_point zoom, fixed_point xBase, fixed_point yBase)
        {
            fixed_point xloc = xBase + x * zoom - 1;
            fixed_point yloc = xBase + x * zoom - 1;
            fixed_point topLeft = interpolate(xloc, yloc);
            fixed_point lowLeft = interpolate(xloc, yloc + zoom);
            fixed_point topRight = interpolate(xloc + zoom, yloc);
            fixed_point lowRight = interpolate(xloc + zoom, yloc + zoom);
            return lowRight - topRight - lowLeft + topLeft;
        }

        private fixed_point interpolate(fixed_point x, fixed_point y)
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

            var sum = inImage(wholeX, wholeY) ? integralImage[wholeX, wholeY] : 0;
            var rt = inImage(wholeX, wholeY + 1) ? integralImage[wholeX, wholeY + 1] - sum : 0;
            var lo = inImage(wholeX + 1, wholeY) ? integralImage[wholeX + 1, wholeY] - sum : 0;
            var lr = inImage(wholeX + 1, wholeY + 1) ? integralImage[wholeX + 1, wholeY + 1] - (sum + rt + lo) : 0;

            return sum + rt * fracX + lo * fracY + lr * fracX * fracY;
        }

        private bool inImage(int x, int y)
        {
            return x >= 0 && y >= 0 && x < integralImage.Width && y < integralImage.Height;
        }

        private static readonly ConcurrentDictionary<string, WeakReference<Bitmap<int>>> intImageCache = new ConcurrentDictionary<string, WeakReference<Bitmap<int>>>();

        public void setSample(ImageSample s)
        {
            if (!this.sample.fileEquals(s))
            {
                var entry = intImageCache.GetOrAdd(s.fileName, new WeakReference<Bitmap<int>>(null));
                lock (entry)
                {
                    if (!entry.TryGetTarget(out integralImage))
                    {
                        var image = new System.Drawing.Bitmap(s.fileName);

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

                        integralImage = new Bitmap<int>(dst, image.Width, image.Height, stride);

                        entry.SetTarget(integralImage);
                    }
                }
            }
            this.sample = s;
        }

        public AdaBoost.ILearner<ImageSample> withParams(string parameters)
        {
            return new LBPImageLearner(Configuration.Parse(parameters), this);
        }

        public string getUniqueIDString()
        {
            return "LBPImageSample";
        }

        public override string ToString()
        {
            return getUniqueIDString() + " [" + config.ToString() + "]";
        }

        public IEnumerable<string> getPossibleParams()
        {
            for (int scaleLevel = 0; scaleLevel < Configuration.numRelativeScaleLevels; scaleLevel++)
            {
                for (int bucket = 0; bucket < (1 << Configuration.numSurroundingPixels); bucket++)
                {
                    yield return new Configuration(scaleLevel * Configuration.scaleChangePerLevel, bucket).ToString();
                }
            }
        }
    }
}

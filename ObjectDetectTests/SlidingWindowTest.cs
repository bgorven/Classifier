using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectDetect;

namespace ObjectDetectTests
{
    [TestClass]
    public class SlidingWindowTest
    {
        [TestMethod]
        public void NearestWindowTest()
        {
            const int w = 640;
            const int h = 480;
            const int minSize = 128;
            const int maxSize = 256;
            var testee = new SlidingWindow(w, h, minSize, maxSize, 3, 7);
            var prevIndex = -1;

            for (var rectSize = minSize - 1; rectSize <= maxSize; rectSize++ )
            {
                var temp = testee.GetNearestWindow(0, 0, rectSize, rectSize);
                if (temp <= prevIndex) continue;
                for (var y = 0; y < h - rectSize; y++)
                {
                    temp = testee.GetNearestWindow(0, y, rectSize, rectSize);
                    if (temp <= prevIndex) continue;
                    for (var x = 0; x < w - rectSize; x++)
                    {
                        var index = testee.GetNearestWindow(x, y, rectSize, rectSize);
                        if (index >= 0)
                        {
                            double xout, yout, wout, hout;
                            Assert.IsTrue(testee.GetWindowDimensions(index, out xout, out yout, out wout, out hout));
                            Assert.IsTrue(Contains(xout, yout, wout, hout, x, y, rectSize, rectSize));
                            prevIndex = index;
                        }
                    }
                }
            }
        }

        private static bool Contains(double xbig, double ybig, double wbig, double hbig, int x, int y, int w, int h)
        {
            return xbig <= x && xbig + wbig >= x + w && ybig <= y && ybig + hbig >= y + h;
        }
    }
}

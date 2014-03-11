using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectDetect;

namespace UnitTestProject
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

            int prevIndex = -1;

            for (int rectSize = minSize; rectSize <= maxSize; rectSize++ )
            {
                if (testee.getNearestWindow(0, 0, rectSize, rectSize) <= prevIndex) continue;
                for (int y = 0; y < h - rectSize; y++)
                {
                    if (testee.getNearestWindow(0, y, rectSize, rectSize) <= prevIndex) continue;
                    for (int x = 0; x < w - rectSize; x++)
                    {
                        int index = testee.getNearestWindow(x, y, rectSize, rectSize);
                        if (index > 0 && index != prevIndex)
                        {
                            Assert.IsTrue(prevIndex < index);
                            prevIndex = index;
                        }
                    }
                }
            }
        }
    }
}

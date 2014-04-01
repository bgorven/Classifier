using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectDetect;

namespace ObjectDetectTests
{
    [TestClass()]
    public class fixed_pointTests
    {

        [TestMethod()]
        public void operatorDoubleTest()
        {
            Assert.AreEqual(3.0, (double)((fixed_point)3.0));
        }

        [TestMethod()]
        public void operatorIntTest()
        {
            Assert.AreEqual(3.0, (double)((fixed_point)3));
        }

        [TestMethod()]
        public void operatorEqaulsTest()
        {
            Assert.IsTrue((fixed_point)3 == (fixed_point)3);
            Assert.IsFalse((fixed_point)3 == (fixed_point)4);
        }

        [TestMethod()]
        public void operatorNotEqaulsTest()
        {
            Assert.IsTrue((fixed_point)3 != (fixed_point)4);
            Assert.IsFalse((fixed_point)3 != (fixed_point)3);
        }

        [TestMethod()]
        public void operatorDivideTest()
        {
            Assert.AreEqual((fixed_point)3 / (fixed_point)2, (fixed_point)1.5);
        }

        [TestMethod()]
        public void operatorMinusTest(fixed_point number, fixed_point subtrahend)
        {
            Assert.AreEqual((fixed_point)4 - (fixed_point)3, (fixed_point)1);
        }

        [TestMethod()]
        public void operatorPlusTest()
        {
            Assert.AreEqual((fixed_point)4 + (fixed_point)3, (fixed_point)7);
        }

        [TestMethod()]
        public void operatorMultTest()
        {
            Assert.AreEqual((fixed_point)4 * (fixed_point)3, (fixed_point)12);
        }

        [TestMethod()]
        public void operatorLShiftTest(fixed_point number, int shift)
        {
            Assert.AreEqual((fixed_point)4 << 3, (fixed_point)(4*8));
        }

        [TestMethod()]
        public void operatorRShiftTest()
        {
            Assert.AreEqual((fixed_point)4 >> 3, (fixed_point)(4.0 / 8.0));
        }

        [TestMethod()]
        public void operatorLTtest()
        {
            Assert.IsTrue((fixed_point)3 < (fixed_point)4);
        }

        [TestMethod()]
        public void operatorGTTest()
        {
            Assert.IsTrue((fixed_point)4 > (fixed_point)3);
        }

        [TestMethod()]
        public void CeilingTest()
        {
            Assert.AreEqual(((fixed_point)3.5).Ceiling(), 4);
        }

        [TestMethod()]
        public void FloorTest()
        {
            Assert.AreEqual(((fixed_point)3.5).Floor(), 3);
        }

        [TestMethod()]
        public void RoundTest()
        {
            Assert.AreEqual(((fixed_point)3.6).Round(), 4);
            Assert.AreEqual(((fixed_point)3.4).Round(), 3);
        }

        [TestMethod()]
        public void MaxTest()
        {
            Assert.AreEqual(fixed_point.Max(3, 4), (fixed_point)4);
        }

        [TestMethod()]
        public void MinTest()
        {
            Assert.AreEqual(fixed_point.Min(3, 4), (fixed_point)3);
        }
    }
}

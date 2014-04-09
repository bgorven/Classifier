using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectDetect;

namespace ObjectDetectTests
{
    [TestClass]
    public class FixedPointTests
    {

        [TestMethod]
        public void OperatorDoubleTest()
        {
            Assert.AreEqual(3.0, (FixedPoint)3.0);
        }

        [TestMethod]
        public void OperatorIntTest()
        {
            Assert.AreEqual(3.0, (FixedPoint)3);
        }

        [TestMethod]
        public void OperatorEqaulsTest()
        {
            Assert.IsTrue(3 == (FixedPoint)3);
            Assert.IsFalse(3 == (FixedPoint)4);
        }

        [TestMethod]
        public void OperatorNotEqaulsTest()
        {
            Assert.IsTrue(3 != (FixedPoint)4);
            Assert.IsFalse(3 != (FixedPoint)3);
        }

        [TestMethod]
        public void OperatorDivideTest()
        {
            Assert.AreEqual(3 / (FixedPoint)2, (FixedPoint)1.5);
        }

        [TestMethod]
        public void OperatorMinusTest(FixedPoint number, FixedPoint subtrahend)
        {
            Assert.AreEqual(4 - (FixedPoint)3, 1);
        }

        [TestMethod]
        public void OperatorPlusTest()
        {
            Assert.AreEqual(4 + (FixedPoint)3, 7);
        }

        [TestMethod]
        public void OperatorMultTest()
        {
            Assert.AreEqual(4 * (FixedPoint)3, 12);
        }

        [TestMethod]
        public void OperatorLShiftTest(FixedPoint number, int shift)
        {
            Assert.AreEqual((FixedPoint)4 << 3, 4*8);
        }

        [TestMethod]
        public void OperatorRShiftTest()
        {
            Assert.AreEqual((FixedPoint)4 >> 3, (FixedPoint)(4.0 / 8.0));
        }

        [TestMethod]
        public void OperatorLTtest()
        {
            Assert.IsTrue(3 < (FixedPoint)4);
            Assert.IsFalse(640 < (FixedPoint)128);
        }

        [TestMethod]
        public void OperatorGtTest()
        {
            Assert.IsTrue(4 > (FixedPoint)3);
        }

        [TestMethod]
        public void CeilingTest()
        {
            Assert.AreEqual(((FixedPoint)3.5).Ceiling(), 4);
        }

        [TestMethod]
        public void FloorTest()
        {
            Assert.AreEqual(((FixedPoint)3.5).Floor(), 3);
        }

        [TestMethod]
        public void RoundTest()
        {
            Assert.AreEqual(((FixedPoint)3.6).Round(), 4);
            Assert.AreEqual(((FixedPoint)3.4).Round(), 3);
        }

        [TestMethod]
        public void MaxTest()
        {
            Assert.AreEqual(FixedPoint.Max(3, 4), 4);
        }

        [TestMethod]
        public void MinTest()
        {
            Assert.AreEqual(FixedPoint.Min(3, 4), 3);
        }
    }
}

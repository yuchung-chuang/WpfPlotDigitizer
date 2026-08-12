using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;
using System.Drawing;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class RectangleDTests
    {
        // ── Construction ──────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_SetsLeftTopWidthHeight()
        {
            var r = new RectangleD(1.0, 2.0, 3.0, 4.0);
            Assert.AreEqual(1.0, r.Left);
            Assert.AreEqual(2.0, r.Top);
            Assert.AreEqual(3.0, r.Width);
            Assert.AreEqual(4.0, r.Height);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_FromRectangle_SetsValuesFromIntRectangle()
        {
            var rect = new Rectangle(5, 6, 7, 8);
            var r = new RectangleD(rect);
            Assert.AreEqual(5.0, r.Left);
            Assert.AreEqual(6.0, r.Top);
            Assert.AreEqual(7.0, r.Width);
            Assert.AreEqual(8.0, r.Height);
        }

        // ── Derived properties ────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void X_ReturnsLeft()
        {
            var r = new RectangleD(3.0, 0.0, 0.0, 0.0);
            Assert.AreEqual(3.0, r.X);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Y_ReturnsTop()
        {
            var r = new RectangleD(0.0, 5.0, 0.0, 0.0);
            Assert.AreEqual(5.0, r.Y);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Right_ReturnsLeftPlusWidth()
        {
            var r = new RectangleD(2.0, 0.0, 8.0, 0.0);
            Assert.AreEqual(10.0, r.Right);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Bottom_ReturnsTopPlusHeight()
        {
            var r = new RectangleD(0.0, 3.0, 0.0, 7.0);
            Assert.AreEqual(10.0, r.Bottom);
        }

        // ── ToRectangle ───────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void ToRectangle_ExactIntValues_ConvertsPrecisely()
        {
            var r = new RectangleD(2.0, 4.0, 6.0, 8.0);
            var rect = r.ToRectangle();
            Assert.AreEqual(2, rect.X);
            Assert.AreEqual(4, rect.Y);
            Assert.AreEqual(6, rect.Width);
            Assert.AreEqual(8, rect.Height);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ToRectangle_FractionalValues_Rounds()
        {
            var r = new RectangleD(1.6, 2.4, 3.5, 4.4);
            var rect = r.ToRectangle();
            Assert.AreEqual(2, rect.X);   // 1.6 rounds to 2
            Assert.AreEqual(2, rect.Y);   // 2.4 rounds to 2
            Assert.AreEqual(4, rect.Width);  // 3.5 rounds to 4 (midpoint rounds to even)
            Assert.AreEqual(4, rect.Height); // 4.4 rounds to 4
        }

        // ── Equality ─────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new RectangleD(1.0, 2.0, 3.0, 4.0);
            var b = new RectangleD(1.0, 2.0, 3.0, 4.0);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_DifferentLeft_ReturnsFalse()
        {
            var a = new RectangleD(1.0, 2.0, 3.0, 4.0);
            var b = new RectangleD(9.0, 2.0, 3.0, 4.0);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_NonRectangleDObject_ReturnsFalse()
        {
            var a = new RectangleD(1.0, 2.0, 3.0, 4.0);
            Assert.IsFalse(a.Equals("not a rect"));
        }

        // ── Operators ─────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void OperatorEqual_EqualValues_ReturnsTrue()
        {
            var a = new RectangleD(1.0, 2.0, 3.0, 4.0);
            var b = new RectangleD(1.0, 2.0, 3.0, 4.0);
            Assert.IsTrue(a == b);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OperatorNotEqual_DifferentValues_ReturnsTrue()
        {
            var a = new RectangleD(1.0, 2.0, 3.0, 4.0);
            var b = new RectangleD(5.0, 6.0, 7.0, 8.0);
            Assert.IsTrue(a != b);
        }

        // ── GetHashCode consistency ───────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetHashCode_EqualValues_ProduceSameHash()
        {
            var a = new RectangleD(1.0, 2.0, 3.0, 4.0);
            var b = new RectangleD(1.0, 2.0, 3.0, 4.0);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}

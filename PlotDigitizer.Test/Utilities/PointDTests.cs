using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class PointDTests
    {
        // ── Construction / properties ─────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_SetsXAndY()
        {
            var pt = new PointD(1.5, 2.5);
            Assert.AreEqual(1.5, pt.X);
            Assert.AreEqual(2.5, pt.Y);
        }

        // ── Equality ──────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new PointD(3.0, 4.0);
            var b = new PointD(3.0, 4.0);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_DifferentX_ReturnsFalse()
        {
            var a = new PointD(3.0, 4.0);
            var b = new PointD(9.0, 4.0);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_DifferentY_ReturnsFalse()
        {
            var a = new PointD(3.0, 4.0);
            var b = new PointD(3.0, 9.0);
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Equals_NonPointDObject_ReturnsFalse()
        {
            var a = new PointD(3.0, 4.0);
            Assert.IsFalse(a.Equals("not a point"));
        }

        // ── Operators ─────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void OperatorEqual_EqualValues_ReturnsTrue()
        {
            var a = new PointD(1.0, 2.0);
            var b = new PointD(1.0, 2.0);
            Assert.IsTrue(a == b);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OperatorNotEqual_DifferentValues_ReturnsTrue()
        {
            var a = new PointD(1.0, 2.0);
            var b = new PointD(3.0, 4.0);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OperatorNotEqual_EqualValues_ReturnsFalse()
        {
            var a = new PointD(1.0, 2.0);
            var b = new PointD(1.0, 2.0);
            Assert.IsFalse(a != b);
        }

        // ── GetHashCode consistency ───────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void GetHashCode_EqualValues_ProduceSameHash()
        {
            var a = new PointD(7.0, 8.0);
            var b = new PointD(7.0, 8.0);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        // ── ToString ─────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void ToString_ReturnsFormattedString()
        {
            var pt = new PointD(1.0, 2.0);
            Assert.AreEqual("PointD {1, 2}", pt.ToString());
        }
    }
}

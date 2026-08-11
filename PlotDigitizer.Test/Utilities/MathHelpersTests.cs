using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;
using System;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class MathHelpersTests
    {
        // ── Clamp ────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Clamp_ValueBetweenMaxAndMin_ReturnsValue()
        {
            Assert.AreEqual(5.0, MathHelpers.Clamp(5.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Clamp_ValueAboveMax_ReturnsMax()
        {
            Assert.AreEqual(10.0, MathHelpers.Clamp(15.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Clamp_ValueBelowMin_ReturnsMin()
        {
            Assert.AreEqual(0.0, MathHelpers.Clamp(-5.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Clamp_MaxAndMinReversed_StillClampsCorrectly()
        {
            // Passing Min > Max: Swap is called internally, result should be same
            Assert.AreEqual(10.0, MathHelpers.Clamp(15.0, 0.0, 10.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Clamp_ValueAtMax_ReturnsMax()
        {
            Assert.AreEqual(10.0, MathHelpers.Clamp(10.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Clamp_ValueAtMin_ReturnsMin()
        {
            Assert.AreEqual(0.0, MathHelpers.Clamp(0.0, 10.0, 0.0));
        }

        // ── Swap ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Swap_TwoDoubles_SwapsValues()
        {
            double x = 3.0, y = 7.0;
            MathHelpers.Swap(ref x, ref y);
            Assert.AreEqual(7.0, x);
            Assert.AreEqual(3.0, y);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Swap_TwoStrings_SwapsValues()
        {
            string a = "hello", b = "world";
            MathHelpers.Swap(ref a, ref b);
            Assert.AreEqual("world", a);
            Assert.AreEqual("hello", b);
        }

        // ── IsIn ─────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_ValueInsideClosedInterval_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.IsIn(5.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_ValueOnLowerBoundary_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.IsIn(0.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_ValueOnUpperBoundary_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.IsIn(10.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_ValueBelowInterval_ReturnsFalse()
        {
            Assert.IsFalse(MathHelpers.IsIn(-1.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_ValueAboveInterval_ReturnsFalse()
        {
            Assert.IsFalse(MathHelpers.IsIn(11.0, 10.0, 0.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_MaxMinReversed_StillWorksCorrectly()
        {
            // passing Min>Max triggers the internal Swap
            Assert.IsTrue(MathHelpers.IsIn(5.0, 0.0, 10.0));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_BoundaryExcluded_ValueOnBoundary_ReturnsFalse()
        {
            Assert.IsFalse(MathHelpers.IsIn(0.0, 10.0, 0.0, excludeBoundary: true));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsIn_BoundaryExcluded_ValueInside_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.IsIn(5.0, 10.0, 0.0, excludeBoundary: true));
        }

        // ── ApproxEqual ───────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void ApproxEqual_SameValues_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.ApproxEqual(3.0, 3.0, 0.001));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ApproxEqual_DifferenceEqualToTolerance_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.ApproxEqual(3.001, 3.0, 0.001));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ApproxEqual_DifferenceExceedsTolerance_ReturnsFalse()
        {
            Assert.IsFalse(MathHelpers.ApproxEqual(3.002, 3.0, 0.001));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ApproxEqual_NegativeDifferenceWithinTolerance_ReturnsTrue()
        {
            Assert.IsTrue(MathHelpers.ApproxEqual(2.999, 3.0, 0.001));
        }

        // ── Distance ─────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Distance_SamePoint_ReturnsZero()
        {
            var pt = new PointD(3.0, 4.0);
            Assert.AreEqual(0.0, MathHelpers.Distance(pt, pt), 1e-10);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Distance_KnownPoints_ReturnsCorrectDistance()
        {
            var pt1 = new PointD(0.0, 0.0);
            var pt2 = new PointD(3.0, 4.0);
            Assert.AreEqual(5.0, MathHelpers.Distance(pt1, pt2), 1e-10);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Distance_IsSymmetric()
        {
            var pt1 = new PointD(1.0, 2.0);
            var pt2 = new PointD(4.0, 6.0);
            Assert.AreEqual(MathHelpers.Distance(pt1, pt2), MathHelpers.Distance(pt2, pt1), 1e-10);
        }

        // ── Enum Add / Contain ────────────────────────────────────────────────

        [Flags]
        private enum TestFlags
        {
            None  = 0,
            Read  = 1,
            Write = 2,
            Exec  = 4,
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Add_TwoFlagValues_ReturnsCombinedFlags()
        {
            var result = TestFlags.Read.Add(TestFlags.Write);
            Assert.AreEqual((ulong)(TestFlags.Read | TestFlags.Write), result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Contain_EnumContainsSubFlag_ReturnsTrue()
        {
            var combined = TestFlags.Read | TestFlags.Write;
            Assert.IsTrue(combined.Contain(TestFlags.Read));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Contain_EnumDoesNotContainFlag_ReturnsFalse()
        {
            Assert.IsFalse(TestFlags.Read.Contain(TestFlags.Write));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Contain_EnumContainsItself_ReturnsTrue()
        {
            Assert.IsTrue(TestFlags.Exec.Contain(TestFlags.Exec));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;
using System.Drawing;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class AxisTextBoxTests
    {
        // ── AxisTextBox (uses System.Drawing.Rectangle — value type default equality) ──

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTextBox_DefaultConstructed_AllRectanglesAreEmpty()
        {
            var atb = new AxisTextBox();
            Assert.AreEqual(Rectangle.Empty, atb.XMax);
            Assert.AreEqual(Rectangle.Empty, atb.YMax);
            Assert.AreEqual(Rectangle.Empty, atb.XMin);
            Assert.AreEqual(Rectangle.Empty, atb.YMin);
            Assert.AreEqual(Rectangle.Empty, atb.XLabel);
            Assert.AreEqual(Rectangle.Empty, atb.YLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTextBox_SetXMax_RetainsValue()
        {
            var atb = new AxisTextBox { XMax = new Rectangle(1, 2, 3, 4) };
            Assert.AreEqual(new Rectangle(1, 2, 3, 4), atb.XMax);
        }

        // ── AxisLimitTextBoxD ────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_DefaultConstructed_AllRectanglesAreDefault()
        {
            var d = new AxisLimitTextBoxD();
            Assert.AreEqual(default(RectangleD), d.XMax);
            Assert.AreEqual(default(RectangleD), d.YMax);
            Assert.AreEqual(default(RectangleD), d.XMin);
            Assert.AreEqual(default(RectangleD), d.YMin);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_EqualValues_EqualsReturnsTrue()
        {
            var a = new AxisLimitTextBoxD
            {
                XMax = new RectangleD(1, 2, 3, 4),
                YMax = new RectangleD(5, 6, 7, 8),
                XMin = new RectangleD(9, 10, 11, 12),
                YMin = new RectangleD(13, 14, 15, 16),
                XLabel = new RectangleD(17, 18, 19, 20),
                YLabel = new RectangleD(21, 22, 23, 24),
            };
            var b = new AxisLimitTextBoxD
            {
                XMax = new RectangleD(1, 2, 3, 4),
                YMax = new RectangleD(5, 6, 7, 8),
                XMin = new RectangleD(9, 10, 11, 12),
                YMin = new RectangleD(13, 14, 15, 16),
                XLabel = new RectangleD(17, 18, 19, 20),
                YLabel = new RectangleD(21, 22, 23, 24),
            };
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_DifferentXMax_EqualsReturnsFalse()
        {
            var a = new AxisLimitTextBoxD { XMax = new RectangleD(1, 2, 3, 4) };
            var b = new AxisLimitTextBoxD { XMax = new RectangleD(9, 9, 9, 9) };
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_EqualsObject_SameValues_ReturnsTrue()
        {
            var a = new AxisLimitTextBoxD { XMax = new RectangleD(1, 2, 3, 4) };
            var b = new AxisLimitTextBoxD { XMax = new RectangleD(1, 2, 3, 4) };
            Assert.IsTrue(a.Equals((object)b));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_EqualsObject_NonAxisLimitTextBoxD_ReturnsFalse()
        {
            var a = new AxisLimitTextBoxD();
            Assert.IsFalse(a.Equals("not the right type"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_OperatorEqual_EqualValues_ReturnsTrue()
        {
            var a = new AxisLimitTextBoxD { YMin = new RectangleD(1, 2, 3, 4) };
            var b = new AxisLimitTextBoxD { YMin = new RectangleD(1, 2, 3, 4) };
            Assert.IsTrue(a == b);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_OperatorNotEqual_DifferentValues_ReturnsTrue()
        {
            var a = new AxisLimitTextBoxD { YMin = new RectangleD(1, 2, 3, 4) };
            var b = new AxisLimitTextBoxD { YMin = new RectangleD(5, 6, 7, 8) };
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitTextBoxD_GetHashCode_EqualValues_SameHash()
        {
            var a = new AxisLimitTextBoxD { XMax = new RectangleD(1, 2, 3, 4) };
            var b = new AxisLimitTextBoxD { XMax = new RectangleD(1, 2, 3, 4) };
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}

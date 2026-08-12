using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class AxisTitleTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void Constructor_SetsXLabelAndYLabel()
        {
            var title = new AxisTitle("X Axis", "Y Axis");
            Assert.AreEqual("X Axis", title.XLabel);
            Assert.AreEqual("Y Axis", title.YLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void XLabel_CanBeSetAfterConstruction()
        {
            var title = new AxisTitle("old", "y");
            title.XLabel = "new";
            Assert.AreEqual("new", title.XLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void YLabel_CanBeSetAfterConstruction()
        {
            var title = new AxisTitle("x", "old");
            title.YLabel = "new";
            Assert.AreEqual("new", title.YLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTitle_DefaultConstructor_LabelsAreNull()
        {
            var title = new AxisTitle();
            Assert.IsNull(title.XLabel);
            Assert.IsNull(title.YLabel);
        }
    }
}

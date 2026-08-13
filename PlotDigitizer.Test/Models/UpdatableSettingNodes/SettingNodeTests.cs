using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Models
{
    /// <summary>
    /// Tests for all setting nodes:
    ///  - <see cref="AxisLocationNode"/>
    ///  - <see cref="AxisTextBoxNode"/>
    ///  - <see cref="AxisLimitNode"/>
    ///  - <see cref="AxisTitleNode"/>
    ///  - <see cref="AxisLogBaseNode"/>
    ///  - <see cref="DataTypeNode"/>
    ///  - <see cref="FilterMinNode"/>
    ///  - <see cref="FilterMaxNode"/>
    ///  - <see cref="DependOnAxisTextBoxNode{TData}"/> (shared base via concrete subclass)
    /// </summary>
    [TestClass]
    public class SettingNodeTests
    {
        private InputImageNode inputImage;
        private AxisLocationNode axisLocation;
        private AxisTextBoxNode axisTextBox;
        private AxisLimitNode axisLimit;
        private AxisTitleNode axisTitle;
        private AxisLogBaseNode axisLogBase;
        private DataTypeNode dataType;
        private FilterMinNode filterMin;
        private FilterMaxNode filterMax;

        [TestInitialize]
        public void OnTestInitialize()
        {
            inputImage = new InputImageNode();
            axisLocation = new AxisLocationNode(inputImage);
            axisTextBox = new AxisTextBoxNode(axisLocation);
            axisLimit = new AxisLimitNode(axisTextBox);
            axisTitle = new AxisTitleNode(axisTextBox);
            axisLogBase = new AxisLogBaseNode(inputImage);
            dataType = new DataTypeNode(inputImage);
            filterMin = new FilterMinNode(inputImage);
            filterMax = new FilterMaxNode(inputImage);
        }

        // ──────────────────────────────────────────────────────────────────
        // FilterMinNode — seeding
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMinNode_AfterConstruction_IsUpdatedTrue()
        {
            Assert.IsTrue(filterMin.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMinNode_AfterConstruction_DataIsSeededValue()
        {
            var expected = new Rgba(0, 0, 0, byte.MaxValue);
            Assert.AreEqual(expected, filterMin.Data);
        }

        // ──────────────────────────────────────────────────────────────────
        // FilterMaxNode — seeding
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMaxNode_AfterConstruction_IsUpdatedTrue()
        {
            Assert.IsTrue(filterMax.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMaxNode_AfterConstruction_DataIsSeededValue()
        {
            var expected = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
            Assert.AreEqual(expected, filterMax.Data);
        }

        // ──────────────────────────────────────────────────────────────────
        // AxisLogBaseNode — seeded IsUpdated = true, data is default
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBaseNode_AfterConstruction_IsUpdatedTrue()
        {
            Assert.IsTrue(axisLogBase.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBaseNode_AfterConstruction_DataIsDefault()
        {
            Assert.AreEqual(default(PointD), axisLogBase.Data);
        }

        // ──────────────────────────────────────────────────────────────────
        // AxisLocationNode — Update() defaults
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLocationNode_AfterConstruction_IsUpdatedFalse()
        {
            Assert.IsFalse(axisLocation.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLocationNode_WhenInputImageUpdated_UpdatesToDefault()
        {
            using var img = new Emgu.CV.Image<Rgba, byte>(4, 4);
            inputImage.Data = img;

            var result = axisLocation.GetUpdatedData();

            Assert.AreEqual(default(RectangleD), result);
        }

        // ──────────────────────────────────────────────────────────────────
        // AxisTextBoxNode — Update() defaults
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTextBoxNode_AfterConstruction_IsUpdatedFalse()
        {
            Assert.IsFalse(axisTextBox.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTextBoxNode_WhenAxisLocationUpdated_UpdatesToDefault()
        {
            axisLocation.Data = new RectangleD(1, 2, 3, 4);

            var result = axisTextBox.GetUpdatedData();

            Assert.AreEqual(default(AxisLimitTextBoxD), result);
        }

        // ──────────────────────────────────────────────────────────────────
        // DataTypeNode — Update() defaults to Continuous (not default(DataType))
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void DataTypeNode_AfterConstruction_IsUpdatedFalse()
        {
            Assert.IsFalse(dataType.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataTypeNode_WhenInputImageUpdated_DefaultsToContinuous()
        {
            using var img = new Emgu.CV.Image<Rgba, byte>(4, 4);
            inputImage.Data = img;

            var result = dataType.GetUpdatedData();

            Assert.AreEqual(DataType.Continuous, result);
        }

        // ──────────────────────────────────────────────────────────────────
        // DependOnAxisTextBoxNode — AxisLimitNode and AxisTitleNode
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitNode_AfterConstruction_IsUpdatedFalse()
        {
            Assert.IsFalse(axisLimit.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimitNode_WhenAxisTextBoxUpdated_UpdatesToDefault()
        {
            axisTextBox.Data = default;

            var result = axisLimit.GetUpdatedData();

            Assert.AreEqual(default(RectangleD), result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTitleNode_AfterConstruction_IsUpdatedFalse()
        {
            Assert.IsFalse(axisTitle.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTitleNode_WhenAxisTextBoxUpdated_UpdatesToDefault()
        {
            axisTextBox.Data = default;

            var result = axisTitle.GetUpdatedData();

            Assert.AreEqual(default(AxisTitle), result);
        }

        // ──────────────────────────────────────────────────────────────────
        // Assigning Data on setting node invalidates downstream
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLocation_WhenDataAssigned_InvalidatesAxisTextBox()
        {
            // Seed axisTextBox into updated state first
            axisLocation.Data = new RectangleD(0, 0, 10, 10);
            axisTextBox.GetUpdatedData();
            Assert.IsTrue(axisTextBox.IsUpdated);

            // Now change axisLocation
            var outdatedRaised = false;
            axisTextBox.Outdated += (s, e) => outdatedRaised = true;

            axisLocation.Data = new RectangleD(1, 1, 5, 5);

            Assert.IsTrue(outdatedRaised);
            Assert.IsFalse(axisTextBox.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisTextBox_WhenDataAssigned_InvalidatesAxisLimit()
        {
            axisTextBox.Data = default;
            axisLimit.GetUpdatedData();
            Assert.IsTrue(axisLimit.IsUpdated);

            var outdatedRaised = false;
            axisLimit.Outdated += (s, e) => outdatedRaised = true;

            axisTextBox.Data = default;

            Assert.IsTrue(outdatedRaised);
            Assert.IsFalse(axisLimit.IsUpdated);
        }

        // ──────────────────────────────────────────────────────────────────
        // FilterMin/FilterMax — reset on new inputImage
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMinNode_WhenInputImageChanges_BecomesOutdatedAndReseeds()
        {
            // Assign a custom value first
            filterMin.Data = new Rgba(50, 50, 50, 255);

            var outdatedRaised = false;
            filterMin.Outdated += (s, e) => outdatedRaised = true;

            using var img = new Emgu.CV.Image<Rgba, byte>(4, 4);
            inputImage.Data = img;

            Assert.IsTrue(outdatedRaised, "FilterMinNode must become outdated when inputImage changes.");

            // After recompute, value resets to seed
            var result = filterMin.GetUpdatedData();
            Assert.AreEqual(new Rgba(0, 0, 0, byte.MaxValue), result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMaxNode_WhenInputImageChanges_BecomesOutdatedAndReseeds()
        {
            filterMax.Data = new Rgba(10, 10, 10, 255);

            var outdatedRaised = false;
            filterMax.Outdated += (s, e) => outdatedRaised = true;

            using var img = new Emgu.CV.Image<Rgba, byte>(4, 4);
            inputImage.Data = img;

            Assert.IsTrue(outdatedRaised);

            var result = filterMax.GetUpdatedData();
            Assert.AreEqual(new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue), result);
        }
    }
}

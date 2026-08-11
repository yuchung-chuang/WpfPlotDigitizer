using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class UpdatableSettingTests
    {
        private InputImageNode inputImageNode;
        private AxisLocationNode axisLocationNode;
        private AxisTextBoxNode axisTextBoxNode;
        private AxisLimitNode axisLimitNode;
        private AxisTitleNode axisTitleNode;
        private AxisLogBaseNode axisLogBaseNode;
        private FilterMinNode filterMinNode;
        private FilterMaxNode filterMaxNode;
        private DataTypeNode dataTypeNode;
        private UpdatableSetting setting;

        [TestInitialize]
        public void OnTestInitialize()
        {
            inputImageNode = new InputImageNode();
            axisLocationNode = new AxisLocationNode(inputImageNode);
            axisTextBoxNode = new AxisTextBoxNode(axisLocationNode);
            axisLimitNode = new AxisLimitNode(axisTextBoxNode);
            axisTitleNode = new AxisTitleNode(axisTextBoxNode);
            axisLogBaseNode = new AxisLogBaseNode(inputImageNode);
            filterMinNode = new FilterMinNode(inputImageNode);
            filterMaxNode = new FilterMaxNode(inputImageNode);
            dataTypeNode = new DataTypeNode(inputImageNode);

            setting = new UpdatableSetting(
                axisLocationNode,
                axisTextBoxNode,
                axisLimitNode,
                axisTitleNode,
                axisLogBaseNode,
                filterMinNode,
                filterMaxNode,
                dataTypeNode);
        }

        // ──────────────────────────────────────────────────────────────────
        // Each property reads through GetUpdatedData and writes to Data
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLocation_WhenSet_ReturnsSameValue()
        {
            var value = new RectangleD(1, 2, 3, 4);
            setting.AxisLocation = value;
            Assert.AreEqual(value, setting.AxisLocation);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLocation_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            setting.PropertyChanged += (s, e) => changedName = e.PropertyName;

            setting.AxisLocation = new RectangleD(1, 2, 3, 4);

            Assert.AreEqual(nameof(setting.AxisLocation), changedName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMin_WhenSet_ReturnsSameValue()
        {
            var value = new Rgba(10, 20, 30, 255);
            setting.FilterMin = value;
            Assert.AreEqual(value, setting.FilterMin);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMin_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            setting.PropertyChanged += (s, e) => changedName = e.PropertyName;

            setting.FilterMin = new Rgba(10, 20, 30, 255);

            Assert.AreEqual(nameof(setting.FilterMin), changedName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMax_WhenSet_ReturnsSameValue()
        {
            var value = new Rgba(200, 200, 200, 255);
            setting.FilterMax = value;
            Assert.AreEqual(value, setting.FilterMax);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMax_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            setting.PropertyChanged += (s, e) => changedName = e.PropertyName;

            setting.FilterMax = new Rgba(200, 200, 200, 255);

            Assert.AreEqual(nameof(setting.FilterMax), changedName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataType_WhenSet_ReturnsSameValue()
        {
            setting.DataType = DataType.Discrete;
            Assert.AreEqual(DataType.Discrete, setting.DataType);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataType_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            setting.PropertyChanged += (s, e) => changedName = e.PropertyName;

            setting.DataType = DataType.Discrete;

            Assert.AreEqual(nameof(setting.DataType), changedName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimit_WhenSet_ReturnsSameValue()
        {
            var value = new RectangleD(0, 0, 100, 100);
            setting.AxisLimit = value;
            Assert.AreEqual(value, setting.AxisLimit);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLimit_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            setting.PropertyChanged += (s, e) => changedName = e.PropertyName;

            setting.AxisLimit = new RectangleD(0, 0, 100, 100);

            Assert.AreEqual(nameof(setting.AxisLimit), changedName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBase_WhenSet_ReturnsSameValue()
        {
            var value = new PointD(10, 10);
            setting.AxisLogBase = value;
            Assert.AreEqual(value, setting.AxisLogBase);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBase_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            setting.PropertyChanged += (s, e) => changedName = e.PropertyName;

            setting.AxisLogBase = new PointD(10, 10);

            Assert.AreEqual(nameof(setting.AxisLogBase), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // PropertyOutdated relay
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void PropertyOutdated_WhenUpstreamOfAxisLocationChanges_IsRelayedWithCorrectName()
        {
            // A node relays Updated as PropertyChanged and Outdated as PropertyOutdated, so
            // AxisLocation only reports outdated when its own upstream (InputImage) changes.
            using var first = new Image<Rgba, byte>(4, 4);
            inputImageNode.Data = first;
            axisLocationNode.GetUpdatedData();

            var outdatedNames = new System.Collections.Generic.List<string>();
            setting.PropertyOutdated += (s, name) => outdatedNames.Add(name);

            using var second = new Image<Rgba, byte>(8, 8);
            inputImageNode.Data = second;

            CollectionAssert.Contains(outdatedNames, nameof(setting.AxisLocation));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PropertyOutdated_WhenAxisLocationIsAssigned_ReportsDownstreamAxisTextBoxNotItself()
        {
            axisLocationNode.Data = new RectangleD(0, 0, 10, 10);
            axisTextBoxNode.GetUpdatedData();

            var outdatedNames = new System.Collections.Generic.List<string>();
            setting.PropertyOutdated += (s, name) => outdatedNames.Add(name);

            axisLocationNode.Data = new RectangleD(1, 1, 5, 5);

            CollectionAssert.Contains(outdatedNames, nameof(setting.AxisTextBox));
            CollectionAssert.DoesNotContain(outdatedNames, nameof(setting.AxisLocation),
                "Assigning a node's Data raises Updated, which the facade relays as PropertyChanged, not PropertyOutdated.");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PropertyOutdated_WhenFilterMinNodeBecomesOutdated_IsRelayedWithCorrectName()
        {
            // Setting inputImage invalidates every node that depends on it, so several
            // PropertyOutdated events fire.  Collect all names; FilterMin must be present.
            var outdatedNames = new System.Collections.Generic.List<string>();
            setting.PropertyOutdated += (s, name) => outdatedNames.Add(name);

            using var img = new Image<Rgba, byte>(4, 4);
            inputImageNode.Data = img;

            CollectionAssert.Contains(outdatedNames, nameof(setting.FilterMin));
        }

        // ──────────────────────────────────────────────────────────────────
        // Initial values from seeded nodes
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMin_WhenReadWithoutInputImage_ReturnsSeededValue()
        {
            // FilterMinNode seeds IsUpdated = true with (0,0,0,255); no inputImage needed
            var result = setting.FilterMin;
            Assert.AreEqual(new Rgba(0, 0, 0, byte.MaxValue), result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FilterMax_WhenReadWithoutInputImage_ReturnsSeededValue()
        {
            var result = setting.FilterMax;
            Assert.AreEqual(new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue), result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void AxisLogBase_WhenReadWithoutInputImage_ReturnsDefault()
        {
            // AxisLogBaseNode sets IsUpdated = true but Data is default
            Assert.AreEqual(default(PointD), setting.AxisLogBase);
        }
    }
}

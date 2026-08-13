using System.Collections.Generic;
using System.ComponentModel;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class UpdatableModelTests
    {
        private FakeImageService imageService;
        private InputImageNode inputImageNode;
        private AxisLocationNode axisLocationNode;
        private CroppedImageNode croppedImageNode;
        private FilterMinNode filterMinNode;
        private FilterMaxNode filterMaxNode;
        private FilteredImageNode filteredImageNode;
        private EdittedImageNode edittedImageNode;
        private DataTypeNode dataTypeNode;
        private DataPointsNode dataPointsNode;
        private AxisTextBoxNode axisTextBoxNode;
        private AxisLimitNode axisLimitNode;
        private AxisLogBaseNode axisLogBaseNode;
        private DataNode dataNode;
        private UpdatableModel model;

        [TestInitialize]
        public void OnTestInitialize()
        {
            imageService = new FakeImageService();
            inputImageNode = new InputImageNode();
            axisLocationNode = new AxisLocationNode(inputImageNode);
            croppedImageNode = new CroppedImageNode(inputImageNode, axisLocationNode, imageService);
            filterMinNode = new FilterMinNode(inputImageNode);
            filterMaxNode = new FilterMaxNode(inputImageNode);
            filteredImageNode = new FilteredImageNode(croppedImageNode, filterMinNode, filterMaxNode, imageService);
            edittedImageNode = new EdittedImageNode(filteredImageNode);
            dataTypeNode = new DataTypeNode(inputImageNode);
            dataPointsNode = new DataPointsNode(edittedImageNode, dataTypeNode, imageService);
            axisTextBoxNode = new AxisTextBoxNode(axisLocationNode);
            axisLimitNode = new AxisLimitNode(axisTextBoxNode);
            axisLogBaseNode = new AxisLogBaseNode(inputImageNode);
            dataNode = new DataNode(edittedImageNode, axisLimitNode, axisLogBaseNode, dataPointsNode, imageService);

            model = new UpdatableModel(
                inputImageNode,
                croppedImageNode,
                filteredImageNode,
                edittedImageNode,
                dataPointsNode,
                dataNode);
        }

        // ──────────────────────────────────────────────────────────────────
        // InputImage property reads through GetUpdatedData and writes to Data
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void InputImage_WhenSet_ReturnsSameImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.InputImage = img;
            Assert.AreSame(img, model.InputImage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void InputImage_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            model.PropertyChanged += (s, e) => changedName = e.PropertyName;

            using var img = new Image<Rgba, byte>(4, 4);
            model.InputImage = img;

            Assert.AreEqual(nameof(model.InputImage), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // CroppedImage property
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void CroppedImage_WhenSet_ReturnsSameImage()
        {
            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;
            Assert.AreSame(img, model.CroppedImage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CroppedImage_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            model.PropertyChanged += (s, e) => changedName = e.PropertyName;

            using var img = new Image<Rgba, byte>(4, 4);
            model.CroppedImage = img;

            Assert.AreEqual(nameof(model.CroppedImage), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // FilteredImage property
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void FilteredImage_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            model.PropertyChanged += (s, e) => changedName = e.PropertyName;

            using var img = new Image<Rgba, byte>(4, 4);
            model.FilteredImage = img;

            Assert.AreEqual(nameof(model.FilteredImage), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // EdittedImage property
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void EdittedImage_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            model.PropertyChanged += (s, e) => changedName = e.PropertyName;

            using var img = new Image<Rgba, byte>(4, 4);
            model.EdittedImage = img;

            Assert.AreEqual(nameof(model.EdittedImage), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // DataPoints property
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void DataPoints_WhenSet_ReturnsSameValue()
        {
            var points = new List<PointD> { new PointD(1, 2) };
            model.DataPoints = points;
            Assert.AreSame(points, model.DataPoints);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataPoints_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            model.PropertyChanged += (s, e) => changedName = e.PropertyName;

            model.DataPoints = new List<PointD>();

            Assert.AreEqual(nameof(model.DataPoints), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // Data property
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Data_WhenSet_ReturnsSameValue()
        {
            var data = new List<PointD> { new PointD(3, 4) };
            model.Data = data;
            Assert.AreSame(data, model.Data);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Data_WhenSet_RaisesPropertyChangedWithCorrectName()
        {
            string changedName = null;
            model.PropertyChanged += (s, e) => changedName = e.PropertyName;

            model.Data = new List<PointD>();

            Assert.AreEqual(nameof(model.Data), changedName);
        }

        // ──────────────────────────────────────────────────────────────────
        // Outdated event relay
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void PropertyOutdated_WhenInputImageNodeBecomesOutdated_IsRelayedWithCorrectName()
        {
            // Assigning croppedImage invalidates the whole downstream chain
            // (FilteredImage, EdittedImage, DataPoints, Data), so several PropertyOutdated
            // events fire.  Collect all names; FilteredImage must be present.
            var outdatedNames = new System.Collections.Generic.List<string>();
            model.PropertyOutdated += (s, name) => outdatedNames.Add(name);

            using var img2 = new Image<Rgba, byte>(4, 4);
            croppedImageNode.Data = img2;

            CollectionAssert.Contains(outdatedNames, nameof(model.FilteredImage));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void PropertyChanged_WhenCroppedImageNodeDataAssigned_EventHasCorrectPropertyName()
        {
            var changedProperties = new List<string>();
            model.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

            using var img = new Image<Rgba, byte>(4, 4);
            croppedImageNode.Data = img;

            CollectionAssert.Contains(changedProperties, nameof(model.CroppedImage));
        }

        // ──────────────────────────────────────────────────────────────────
        // GetUpdatedData is called through the facade (lazy compute)
        // ──────────────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void InputImage_WhenReadWithoutSet_ReturnsNull()
        {
            Assert.IsNull(model.InputImage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CroppedImage_WhenReadWithoutInputImage_ReturnsNull()
        {
            Assert.IsNull(model.CroppedImage);
        }
    }
}

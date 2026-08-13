using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace PlotDigitizer.Core.Tests.Models
{
    /// <summary>
    /// Tests the plain <see cref="Model"/> facade — auto-properties and
    /// Fody PropertyChanged weaving.
    /// </summary>
    [TestClass]
    public class ModelPropertyTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void Model_SetInputImage_PropertyChangedRaised()
        {
            var model = new Model();
            string raised = null;
            model.PropertyChanged += (s, e) => raised = e.PropertyName;

            using var img = new Image<Rgba, byte>(1, 1);
            model.InputImage = img;

            Assert.AreEqual(nameof(Model.InputImage), raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_SetCroppedImage_PropertyChangedRaised()
        {
            var model = new Model();
            string raised = null;
            model.PropertyChanged += (s, e) => raised = e.PropertyName;

            using var img = new Image<Rgba, byte>(1, 1);
            model.CroppedImage = img;

            Assert.AreEqual(nameof(Model.CroppedImage), raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_SetFilteredImage_PropertyChangedRaised()
        {
            var model = new Model();
            string raised = null;
            model.PropertyChanged += (s, e) => raised = e.PropertyName;

            using var img = new Image<Rgba, byte>(1, 1);
            model.FilteredImage = img;

            Assert.AreEqual(nameof(Model.FilteredImage), raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_SetEdittedImage_PropertyChangedRaised()
        {
            var model = new Model();
            string raised = null;
            model.PropertyChanged += (s, e) => raised = e.PropertyName;

            using var img = new Image<Rgba, byte>(1, 1);
            model.EdittedImage = img;

            Assert.AreEqual(nameof(Model.EdittedImage), raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_SetDataPoints_PropertyChangedRaised()
        {
            var model = new Model();
            string raised = null;
            model.PropertyChanged += (s, e) => raised = e.PropertyName;

            model.DataPoints = new[] { new PointD(1, 2) };

            Assert.AreEqual(nameof(Model.DataPoints), raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_SetData_PropertyChangedRaised()
        {
            var model = new Model();
            string raised = null;
            model.PropertyChanged += (s, e) => raised = e.PropertyName;

            model.Data = new[] { new PointD(3, 4) };

            Assert.AreEqual(nameof(Model.Data), raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_GetInputImage_ReturnsWhatWasSet()
        {
            var model = new Model();
            using var img = new Image<Rgba, byte>(2, 2);
            model.InputImage = img;

            Assert.AreSame(img, model.InputImage);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Model_InitialInputImage_IsNull()
        {
            var model = new Model();
            Assert.IsNull(model.InputImage);
        }
    }
}

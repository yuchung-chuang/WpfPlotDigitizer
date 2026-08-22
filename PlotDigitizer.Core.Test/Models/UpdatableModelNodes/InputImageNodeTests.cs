using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class InputImageNodeTests
    {
        private InputImageNode node;

        [TestInitialize]
        public void OnTestInitialize()
        {
            node = new InputImageNode();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsUpdated_WhenDataIsNull_ReturnsFalse()
        {
            // Data is null by default after construction
            Assert.IsFalse(node.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsUpdated_AfterAssigningNonNullData_ReturnsTrue()
        {
            using var image = new Image<Rgba, byte>(4, 4);
            node.Data = image;
            Assert.IsTrue(node.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsUpdated_AfterAssigningNullData_ReturnsFalse()
        {
            using var image = new Image<Rgba, byte>(4, 4);
            node.Data = image;      // set non-null first
            node.Data = null;       // then null again
            Assert.IsFalse(node.IsUpdated);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_WhenDataIsNull_ReturnsNull()
        {
            var result = node.GetUpdatedData();
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void GetUpdatedData_AfterAssigningImage_ReturnsThatImage()
        {
            using var image = new Image<Rgba, byte>(4, 4);
            node.Data = image;
            var result = node.GetUpdatedData();
            Assert.AreSame(image, result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataSetter_WhenAssigningNonNullImage_RaisesUpdatedEvent()
        {
            using var image = new Image<Rgba, byte>(4, 4);
            var raised = false;
            node.Updated += (s, e) => raised = true;
            node.Data = image;
            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DataSetter_WhenAssigningNullAfterImage_RaisesUpdatedEvent()
        {
            using var image = new Image<Rgba, byte>(4, 4);
            node.Data = image;

            var raised = false;
            node.Updated += (s, e) => raised = true;
            node.Data = null;
            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsUpdated_IsOverride_NotDrivenByBaseIsUpdatedField()
        {
            // The base field IsUpdated can be set to true, but InputImageNode.IsUpdated
            // always reflects Data != null, regardless of the backing field.
            using var image = new Image<Rgba, byte>(4, 4);
            node.Data = image;     // sets backing field via OnUpdated
            Assert.IsTrue(node.IsUpdated);

            // If we assign null, IsUpdated must track Data even if base field is true
            node.Data = null;
            Assert.IsFalse(node.IsUpdated, "IsUpdated must be false when Data is null, even if base field was set true.");
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;
using System.Drawing;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class DropEventArgsTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void DropEventArgs_DropType_FileValue_IsZero()
        {
            Assert.AreEqual(0, (int)DropEventArgs.DropType.File);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DropEventArgs_SetFileProperties_RetainsValues()
        {
            var args = new DropEventArgs
            {
                Type = DropEventArgs.DropType.File,
                FileName = "test.png"
            };
            Assert.AreEqual(DropEventArgs.DropType.File, args.Type);
            Assert.AreEqual("test.png", args.FileName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DropEventArgs_SetUrlProperties_RetainsValues()
        {
            var uri = new System.Uri("http://example.com/img.png");
            var args = new DropEventArgs
            {
                Type = DropEventArgs.DropType.Url,
                Url = uri
            };
            Assert.AreEqual(DropEventArgs.DropType.Url, args.Type);
            Assert.AreEqual(uri, args.Url);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DropEventArgs_SetImageProperty_RetainsValue()
        {
            using var bmp = new Bitmap(1, 1);
            var args = new DropEventArgs
            {
                Type = DropEventArgs.DropType.Image,
                Image = bmp
            };
            Assert.AreEqual(DropEventArgs.DropType.Image, args.Type);
            Assert.AreSame(bmp, args.Image);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void DropType_HasThreeMembers()
        {
            var names = System.Enum.GetNames(typeof(DropEventArgs.DropType));
            Assert.AreEqual(3, names.Length);
        }
    }
}

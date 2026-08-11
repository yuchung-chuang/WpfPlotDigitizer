using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class OcrSettingsTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void OcrSettings_DefaultInstance_AllPropertiesNull()
        {
            var sut = new OcrSettings();
            Assert.IsNull(sut.DataPath);
            Assert.IsNull(sut.Language);
            Assert.IsNull(sut.WhiteList);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OcrSettings_SetDataPath_ValueIsStored()
        {
            var sut = new OcrSettings { DataPath = "/usr/share/tessdata" };
            Assert.AreEqual("/usr/share/tessdata", sut.DataPath);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OcrSettings_SetLanguage_ValueIsStored()
        {
            var sut = new OcrSettings { Language = "eng" };
            Assert.AreEqual("eng", sut.Language);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OcrSettings_SetWhiteList_ValueIsStored()
        {
            var sut = new OcrSettings { WhiteList = "0123456789" };
            Assert.AreEqual("0123456789", sut.WhiteList);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class FileDialogResultsTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void FileDialogResults_ConstructedWithValidArgs_PropertiesAreSet()
        {
            var result = new FileDialogResults("file.csv", true);
            Assert.AreEqual("file.csv", result.FileName);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FileDialogResults_ConstructedWithHasFileFalse_IsValidFalse()
        {
            var result = new FileDialogResults("", false);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FileDialogResults_FileNameIsSettable()
        {
            var result = new FileDialogResults("old.csv", true);
            result.FileName = "new.csv";
            Assert.AreEqual("new.csv", result.FileName);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FileDialogResults_IsValidIsSettable()
        {
            var result = new FileDialogResults("file.csv", true);
            result.IsValid = false;
            Assert.IsFalse(result.IsValid);
        }
    }
}

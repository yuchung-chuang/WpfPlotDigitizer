using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class StringExtensionsTests
    {
        // ── Accepted schemes ──────────────────────────────────────────────────

        [DataTestMethod]
        [TestCategory("Unit")]
        [DataRow("http://example.com/page")]
        [DataRow("https://example.com/page")]
        [DataRow("ftp://files.example.com/file.txt")]
        public void ToUri_AcceptedScheme_ReturnsNonNullUri(string url)
        {
            var result = url.ToUri();
            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [TestCategory("Unit")]
        [DataRow("http://example.com/page", "http")]
        [DataRow("https://example.com/page", "https")]
        [DataRow("ftp://files.example.com/file.txt", "ftp")]
        public void ToUri_AcceptedScheme_ReturnsUriWithCorrectScheme(string url, string expectedScheme)
        {
            var result = url.ToUri();
            Assert.AreEqual(expectedScheme, result.Scheme);
        }

        // ── Rejected schemes / inputs ─────────────────────────────────────────

        [DataTestMethod]
        [TestCategory("Unit")]
        [DataRow("file:///C:/Users/test.txt")]
        [DataRow("mailto:user@example.com")]
        [DataRow("ssh://host.example.com")]
        public void ToUri_OtherSchemes_ReturnsNull(string url)
        {
            Assert.IsNull(url.ToUri());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ToUri_RelativePath_ReturnsNull()
        {
            Assert.IsNull("relative/path/page".ToUri());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ToUri_GarbageString_ReturnsNull()
        {
            Assert.IsNull("not a url at all!@@#".ToUri());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ToUri_EmptyString_ReturnsNull()
        {
            Assert.IsNull("".ToUri());
        }
    }
}

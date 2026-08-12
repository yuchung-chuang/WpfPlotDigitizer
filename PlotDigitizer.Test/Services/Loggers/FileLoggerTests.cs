using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class FileLoggerTests
    {
        private string _tempDir;

        [TestInitialize]
        public void Init()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"FileLoggerTest_{Guid.NewGuid():N}");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_CreatesDirectoryIfMissing()
        {
            Assert.IsFalse(Directory.Exists(_tempDir));
            var logger = new FileLogger(_tempDir);
            logger.Log<string>(LogLevel.Information, default, "msg", null, (s, _) => s);
            Assert.IsTrue(Directory.Exists(_tempDir));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_CreatesDateNamedLogFile()
        {
            var logger = new FileLogger(_tempDir);
            logger.Log<string>(LogLevel.Information, default, "hello", null, (s, _) => s);

            var expectedFile = Path.Combine(_tempDir, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
            Assert.IsTrue(File.Exists(expectedFile));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_AppendsMsgToFile()
        {
            var logger = new FileLogger(_tempDir);
            logger.Log<string>(LogLevel.Information, default, "first",  null, (s, _) => s);
            logger.Log<string>(LogLevel.Warning,     default, "second", null, (s, _) => s);

            var file = Path.Combine(_tempDir, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
            var content = File.ReadAllText(file);
            Assert.IsTrue(content.Contains("first"));
            Assert.IsTrue(content.Contains("second"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_NullFormatter_DoesNotCreateFile()
        {
            var logger = new FileLogger(_tempDir);
            logger.Log<string>(LogLevel.Information, default, "state", null, null);
            Assert.IsFalse(Directory.Exists(_tempDir));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_WithException_IncludesExceptionTypeInFile()
        {
            var logger = new FileLogger(_tempDir);
            var ex = new InvalidOperationException("test-boom");
            logger.Log<string>(LogLevel.Error, default, "msg", ex, (s, _) => s);

            var file = Path.Combine(_tempDir, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
            var content = File.ReadAllText(file);
            Assert.IsTrue(content.Contains("InvalidOperationException"), $"Content was:\n{content}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_WithException_IncludesExceptionMessageInFile()
        {
            var logger = new FileLogger(_tempDir);
            var ex = new InvalidOperationException("test-boom");
            logger.Log<string>(LogLevel.Error, default, "msg", ex, (s, _) => s);

            var file = Path.Combine(_tempDir, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
            var content = File.ReadAllText(file);
            Assert.IsTrue(content.Contains("test-boom"), $"Content was:\n{content}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_AnyLogLevel_ReturnsTrue()
        {
            var logger = new FileLogger(_tempDir);
            Assert.IsTrue(logger.IsEnabled(LogLevel.Critical));
        }
    }

    [TestClass]
    public class FileLoggerProviderTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void CreateLogger_ReturnsFileLoggerInstance()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), $"FLPTest_{Guid.NewGuid():N}");
            try {
                var provider = new FileLoggerProvider(tempDir);
                var logger = provider.CreateLogger("cat");
                Assert.IsInstanceOfType(logger, typeof(FileLogger));
            } finally {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void FileLoggerProvider_HasProviderAliasFileAttribute()
        {
            var attrs = typeof(FileLoggerProvider).GetCustomAttributes(typeof(Microsoft.Extensions.Logging.ProviderAliasAttribute), inherit: false);
            Assert.AreEqual(1, attrs.Length);
            var alias = (Microsoft.Extensions.Logging.ProviderAliasAttribute)attrs[0];
            Assert.AreEqual("File", alias.Alias);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Dispose_DoesNotThrow()
        {
            var provider = new FileLoggerProvider(Path.GetTempPath());
            provider.Dispose();
        }
    }
}

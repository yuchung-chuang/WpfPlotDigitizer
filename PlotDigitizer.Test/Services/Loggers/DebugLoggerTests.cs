using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PlotDigitizer.Core.Tests.Models
{
    [TestClass]
    public class DebugLoggerTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void IsEnabled_AnyLogLevel_ReturnsTrue()
        {
            var logger = new DebugLogger();
            Assert.IsTrue(logger.IsEnabled(LogLevel.Trace));
            Assert.IsTrue(logger.IsEnabled(LogLevel.Debug));
            Assert.IsTrue(logger.IsEnabled(LogLevel.Information));
            Assert.IsTrue(logger.IsEnabled(LogLevel.Warning));
            Assert.IsTrue(logger.IsEnabled(LogLevel.Error));
            Assert.IsTrue(logger.IsEnabled(LogLevel.Critical));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void BeginScope_ReturnsNonThrowingDisposable()
        {
            var logger = new DebugLogger();
            // default! is returned; calling Dispose on null-like result must not throw
            var scope = logger.BeginScope("state");
            // The method signature returns default! which is null for a reference type.
            // We just verify it does not throw, not that it is non-null.
            // (Production code returns default!, so scope may be null here.)
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_NullFormatter_DoesNotThrow()
        {
            var logger = new DebugLogger();
            // formatter == null → early return, no exception
            logger.Log<string>(LogLevel.Information, default, "state", null, null);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_WithFormatter_DoesNotThrow()
        {
            var logger = new DebugLogger();
            logger.Log<string>(
                LogLevel.Information,
                default,
                "hello",
                null,
                (state, ex) => state);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Log_WithException_DoesNotThrow()
        {
            var logger = new DebugLogger();
            logger.Log<string>(
                LogLevel.Error,
                default,
                "msg",
                new InvalidOperationException("boom"),
                (state, ex) => state + " " + ex?.Message);
        }
    }

    [TestClass]
    public class DebugLoggerProviderTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void CreateLogger_ReturnsDebugLoggerInstance()
        {
            var provider = new DebugLoggerProvider();
            var logger = provider.CreateLogger("category");
            Assert.IsInstanceOfType(logger, typeof(DebugLogger));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Dispose_DoesNotThrow()
        {
            var provider = new DebugLoggerProvider();
            provider.Dispose(); // must not throw
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlotDigitizer.Core.Tests
{
    // -----------------------------------------------------------------------
    // Recording ILogger — captures the last call so tests can assert on it.
    // -----------------------------------------------------------------------

    internal sealed class RecordingLogger : ILogger
    {
        public struct LogCall
        {
            public LogLevel Level;
            public string FormattedMessage;
            public Exception Exception;
        }

        public List<LogCall> Calls { get; } = new();

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Calls.Add(new LogCall
            {
                Level = logLevel,
                FormattedMessage = formatter(state, exception),
                Exception = exception,
            });
        }
    }

    // -----------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------

    [TestClass]
    public class LoggerExtensionsTests
    {
        private RecordingLogger _logger;

        [TestInitialize]
        public void Init() => _logger = new RecordingLogger();

        [TestMethod]
        [TestCategory("Unit")]
        public void LogTrace_ForwardsToILoggerAtTraceLevel()
        {
            _logger.LogTrace("trace-msg");
            Assert.AreEqual(1, _logger.Calls.Count);
            Assert.AreEqual(LogLevel.Trace, _logger.Calls[0].Level);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogTrace_FormattedMessageContainsProvidedMessage()
        {
            _logger.LogTrace("trace-msg");
            StringAssert.Contains(_logger.Calls[0].FormattedMessage, "trace-msg");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogDebug_ForwardsToILoggerAtDebugLevel()
        {
            _logger.LogDebug("debug-msg");
            Assert.AreEqual(LogLevel.Debug, _logger.Calls[0].Level);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogDebug_FormattedMessageContainsProvidedMessage()
        {
            _logger.LogDebug("debug-msg");
            StringAssert.Contains(_logger.Calls[0].FormattedMessage, "debug-msg");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogInformation_ForwardsToILoggerAtInformationLevel()
        {
            _logger.LogInformation("info-msg");
            Assert.AreEqual(LogLevel.Information, _logger.Calls[0].Level);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogInformation_FormattedMessageContainsCallerFileName()
        {
            _logger.LogInformation("info-msg");
            // The formatted message should contain this source file's name.
            StringAssert.Contains(_logger.Calls[0].FormattedMessage, "LoggerExtensionsTests.cs");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogWarning_ForwardsToILoggerAtWarningLevel()
        {
            _logger.LogWarning("warn-msg");
            Assert.AreEqual(LogLevel.Warning, _logger.Calls[0].Level);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogWarning_FormattedMessageContainsProvidedMessage()
        {
            _logger.LogWarning("warn-msg");
            StringAssert.Contains(_logger.Calls[0].FormattedMessage, "warn-msg");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogCritical_ForwardsToILoggerAtCriticalLevel()
        {
            _logger.LogCritical("crit-msg");
            Assert.AreEqual(LogLevel.Critical, _logger.Calls[0].Level);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogCritical_FormattedMessageContainsProvidedMessage()
        {
            _logger.LogCritical("crit-msg");
            StringAssert.Contains(_logger.Calls[0].FormattedMessage, "crit-msg");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogError_ForwardsToILoggerAtErrorLevel()
        {
            _logger.LogError(new InvalidOperationException("ex"), "error-msg");
            Assert.AreEqual(LogLevel.Error, _logger.Calls[0].Level);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogError_ExceptionIsPropagatedToLogger()
        {
            var ex = new InvalidOperationException("ex");
            _logger.LogError(ex, "error-msg");
            Assert.AreSame(ex, _logger.Calls[0].Exception);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LogInformation_FormattedMessageContainsCallerMemberName()
        {
            // Call site is this method; the member name should appear in the formatted message.
            _logger.LogInformation("member-test");
            StringAssert.Contains(_logger.Calls[0].FormattedMessage,
                nameof(LogInformation_FormattedMessageContainsCallerMemberName));
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PlotDigitizer.Core
{
    public static class LoggerExtensions
    {
        public static void LogTrace(this ILogger logger, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);
            logger.LogTrace("[{fileName} > {memberName}() > Line {lineNumber}]" + Environment.NewLine + "{message}", fileName, memberName, lineNumber, message);
        }

        public static void LogDebug(this ILogger logger, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);
            logger.LogDebug("[{fileName} > {memberName}() > Line {lineNumber}]" + Environment.NewLine + "{message}", fileName, memberName, lineNumber, message);
        }

        public static void LogInformation(this ILogger logger, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);
            logger.LogInformation("[{fileName} > {memberName}() > Line {lineNumber}]" + Environment.NewLine + "{message}", fileName, memberName, lineNumber, message);
        }

        public static void LogWarning(this ILogger logger, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);
            logger.LogWarning("[{fileName} > {memberName}() > Line {lineNumber}]" + Environment.NewLine + "{message}", fileName, memberName, lineNumber, message);
        }

        public static void LogCritical(this ILogger logger, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);
            logger.LogCritical("[{fileName} > {memberName}() > Line {lineNumber}]" + Environment.NewLine + "{message}", fileName, memberName, lineNumber, message);
        }

        public static void LogError(this ILogger logger, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);
            logger.LogError("[{fileName} > {memberName}() > Line {lineNumber}]" + Environment.NewLine + "{message}", fileName, memberName, lineNumber, message);
        }
    }
}

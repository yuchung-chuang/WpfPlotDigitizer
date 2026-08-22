using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Services
{
    [TestClass]
    public class OcrServiceTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void Ocr_MissingDataPath_ReturnsEmptyTextWithoutThrowing()
        {
            var missingDataPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));
            var optionsFactory = new FixedOptionsFactory(new OcrSettings
            {
                DataPath = missingDataPath
            });

            var service = new OcrService("Numerical", optionsFactory);

            Assert.AreEqual(string.Empty, service.Ocr(null));
        }

        private sealed class FixedOptionsFactory : IOptionsFactory<OcrSettings>
        {
            private readonly OcrSettings settings;

            public FixedOptionsFactory(OcrSettings settings)
            {
                this.settings = settings;
            }

            public OcrSettings Create(string name) => settings;

            public OcrSettings Create(string name, Action<OcrSettings> configure)
            {
                var result = new OcrSettings
                {
                    DataPath = settings.DataPath,
                    Language = settings.Language,
                    WhiteList = settings.WhiteList
                };
                configure(result);
                return result;
            }
        }
    }
}
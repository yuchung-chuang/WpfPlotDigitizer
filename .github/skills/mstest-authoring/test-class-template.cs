// Template: MSTest test class for PlotDigitizer.
// Copy into PlotDigitizer.Core.Test/<mirrored source folder>/ and adapt.
//
// Reminders:
//  - Exactly one [TestCategory]: "Unit", "Integration", "EndToEnd", or "UI".
//  - Name tests Method_Scenario_ExpectedOutcome.
//  - Parameterized tests should use [DataTestMethod]; the adapter also honours [DataRow]
//    on a plain [TestMethod], but [DataTestMethod] states the intent.
//  - Pass null for ILogger<T>; production code calls logger?.Log...
//  - Dispose Image<Rgba, byte> instances you allocate.

using System;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core;

namespace PlotDigitizer.Core.Tests
{
    [TestClass]
    public class ExampleUnitTests
    {
        private EditService<string> service;

        [TestInitialize]
        public void OnTestInitialize()
        {
            service = new EditService<string>();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanUndo_BeforeInitialise_ReturnsFalse()
        {
            Assert.IsFalse(service.CanUndo());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void Edit_AfterUndo_DiscardsRedoBranch()
        {
            service.Initialise("a");
            service.Edit(("b", "second"));
            service.Undo();

            service.Edit(("c", "third"));

            Assert.AreEqual("c", service.CurrentObject);
            Assert.IsFalse(service.CanRedo(), "Editing after an undo must truncate the redo branch.");
        }

        // Value tables: [DataTestMethod] makes the intent explicit.
        [DataTestMethod]
        [TestCategory("Unit")]
        [DataRow(5.0, 10.0, 0.0, 5.0)]
        [DataRow(15.0, 10.0, 0.0, 10.0)]
        [DataRow(-1.0, 10.0, 0.0, 0.0)]
        [DataRow(5.0, 0.0, 10.0, 5.0)]   // Max/Min supplied reversed: Clamp swaps them
        public void Clamp_WithBounds_ReturnsExpectedValue(double value, double max, double min, double expected)
        {
            Assert.AreEqual(expected, MathHelpers.Clamp(value, max, min));
        }
    }

    [TestClass]
    public class ExampleIntegrationTests
    {
        private ServiceProvider provider;

        [TestInitialize]
        public void OnTestInitialize()
        {
            // Compose the way production does, so a missing registration fails the test.
            provider = new ServiceCollection()
                .AddTransient<IImageService, EmguCvService>()
                .AddModel()
                .AddViewModels()
                .BuildServiceProvider();
        }

        [TestCleanup]
        public void OnTestCleanup() => provider?.Dispose();

        [TestMethod]
        [TestCategory("Integration")]
        public void Model_ResolvedTwice_ReturnsSameSingletonInstance()
        {
            Assert.AreSame(
                provider.GetRequiredService<Model>(),
                provider.GetRequiredService<Model>());
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void CroppedImage_WhenAxisLocationChanges_IsRecomputed()
        {
            var model = provider.GetRequiredService<Model>();
            var setting = provider.GetRequiredService<Setting>();

            // Load directly from the output directory: no WPF pack URI, no STA requirement.
            using var image = new Image<Rgba, byte>("Assets/data.png");
            model.InputImage = image;
            setting.AxisLocation = new RectangleD(10, 10, 100, 100);

            var first = model.CroppedImage;
            setting.AxisLocation = new RectangleD(20, 20, 120, 120);
            var second = model.CroppedImage;

            Assert.AreNotSame(first, second, "Changing AxisLocation must invalidate CroppedImage.");
            Assert.AreEqual(120, second.Width);
        }
    }
}

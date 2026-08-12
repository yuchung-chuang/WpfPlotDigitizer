using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core;
using System.Text.Json;

namespace PlotDigitizer.Core.Tests.Utilities
{
    [TestClass]
    public class RgbaConverterTests
    {
        private static JsonSerializerOptions BuildOptions()
        {
            var opts = new JsonSerializerOptions();
            opts.Converters.Add(new RgbaConverter());
            return opts;
        }

        // ── Round-trip ────────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void RoundTrip_TypicalRgba_PreservesAllChannels()
        {
            var opts = BuildOptions();
            var original = new Rgba(100, 150, 200, 255);
            var json = JsonSerializer.Serialize(original, opts);
            var restored = JsonSerializer.Deserialize<Rgba>(json, opts);
            Assert.AreEqual(original.Red,   restored.Red,   1e-9);
            Assert.AreEqual(original.Green, restored.Green, 1e-9);
            Assert.AreEqual(original.Blue,  restored.Blue,  1e-9);
            Assert.AreEqual(original.Alpha, restored.Alpha, 1e-9);
        }

        // ── Channel order in serialised JSON ─────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Write_ChannelOrder_IsRedGreenBlueAlpha()
        {
            var opts = BuildOptions();
            var rgba = new Rgba(10, 20, 30, 40);
            var json = JsonSerializer.Serialize(rgba, opts);
            int ri = json.IndexOf("Red");
            int gi = json.IndexOf("Green");
            int bi = json.IndexOf("Blue");
            int ai = json.IndexOf("Alpha");
            Assert.IsTrue(ri < gi, "Red should appear before Green");
            Assert.IsTrue(gi < bi, "Green should appear before Blue");
            Assert.IsTrue(bi < ai, "Blue should appear before Alpha");
        }

        // ── Read with properties in different order ───────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void Read_PropertiesInReverseOrder_StillRestoresCorrectly()
        {
            var opts = BuildOptions();
            var json = "{\"Alpha\":255,\"Blue\":3,\"Green\":2,\"Red\":1}";
            var rgba = JsonSerializer.Deserialize<Rgba>(json, opts);
            Assert.AreEqual(1.0, rgba.Red,   1e-9);
            Assert.AreEqual(2.0, rgba.Green, 1e-9);
            Assert.AreEqual(3.0, rgba.Blue,  1e-9);
            Assert.AreEqual(255.0, rgba.Alpha, 1e-9);
        }

        // ── Zero values ───────────────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void RoundTrip_AllZeroChannels_ReturnsAllZero()
        {
            var opts = BuildOptions();
            var original = new Rgba(0, 0, 0, 0);
            var json = JsonSerializer.Serialize(original, opts);
            var restored = JsonSerializer.Deserialize<Rgba>(json, opts);
            Assert.AreEqual(0.0, restored.Red,   1e-9);
            Assert.AreEqual(0.0, restored.Green, 1e-9);
            Assert.AreEqual(0.0, restored.Blue,  1e-9);
            Assert.AreEqual(0.0, restored.Alpha, 1e-9);
        }

        // ── Fractional/out-of-range values ───────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void RoundTrip_FractionalValues_PreservesValues()
        {
            // Rgba stores channels as double; fractional values round-trip fine
            var opts = BuildOptions();
            var json = "{\"Red\":1.5,\"Green\":2.75,\"Blue\":0.25,\"Alpha\":128.0}";
            var rgba = JsonSerializer.Deserialize<Rgba>(json, opts);
            Assert.AreEqual(1.5,   rgba.Red,   1e-9);
            Assert.AreEqual(2.75,  rgba.Green, 1e-9);
            Assert.AreEqual(0.25,  rgba.Blue,  1e-9);
            Assert.AreEqual(128.0, rgba.Alpha, 1e-9);
        }
    }
}

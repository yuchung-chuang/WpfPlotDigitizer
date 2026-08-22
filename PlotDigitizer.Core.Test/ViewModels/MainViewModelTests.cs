using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.Core.Tests.Fakes;

namespace PlotDigitizer.Core.Tests.ViewModels
{
    [TestClass]
    public class MainViewModelTests
    {
        private Model model;
        private Setting setting;
        private FakeFileDialogService fileDialog;
        private FakeMessageBoxService messageBox;
        private FakePageService pageService;
        private MainViewModel vm;
        private string tempDir;

        [TestInitialize]
        public void Setup()
        {
            model = new Model();
            setting = new Setting();
            fileDialog = new FakeFileDialogService();
            messageBox = new FakeMessageBoxService();
            pageService = new FakePageService();
            vm = new MainViewModel(model, setting, fileDialog, messageBox, pageService, null);
            tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }

        // ── LoadSetting — cancelled dialog ─────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WhenDialogCancelled_ShowsNoMessage()
        {
            fileDialog.OpenResult = new FileDialogResults(null, false);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(0, messageBox.OkMessages.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WhenDialogCancelled_DoesNotModifySetting()
        {
            fileDialog.OpenResult = new FileDialogResults(null, false);
            setting.AxisLogBase = new PointD(5, 10);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(new PointD(5, 10), setting.AxisLogBase);
        }

        // ── LoadSetting — JSON round-trip ──────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WithJsonFile_UpdatesSettingFromFile()
        {
            // Write a JSON file with known FilterMax
            var jsonPath = Path.Combine(tempDir, "setting.json");
            var source = new Setting { FilterMax = new Rgba(100, 150, 200, 255) };
            var json = JsonSerializer.Serialize(source, new JsonSerializerOptions
            {
                Converters = { new RgbaConverter() }
            });
            File.WriteAllText(jsonPath, json);

            fileDialog.OpenResult = new FileDialogResults(jsonPath, true);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(100.0, setting.FilterMax.Red);
            Assert.AreEqual(150.0, setting.FilterMax.Green);
            Assert.AreEqual(200.0, setting.FilterMax.Blue);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WithJsonFile_ShowsSuccessMessage()
        {
            var jsonPath = Path.Combine(tempDir, "setting.json");
            var source = new Setting();
            var json = JsonSerializer.Serialize(source, new JsonSerializerOptions
            {
                Converters = { new RgbaConverter() }
            });
            File.WriteAllText(jsonPath, json);
            fileDialog.OpenResult = new FileDialogResults(jsonPath, true);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(1, messageBox.OkMessages.Count);
            Assert.IsTrue(messageBox.OkMessages[0].message.Contains("successfully"));
        }

        // ── LoadSetting — XML round-trip ───────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WithXmlFile_UpdatesSettingFromFile()
        {
            var xmlPath = Path.Combine(tempDir, "setting.xml");
            var source = new Setting { AxisLogBase = new PointD(10, 2) };
            var xmlSerializer = new XmlSerializer(typeof(Setting));
            using (var stream = new FileStream(xmlPath, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, source.Copy());
            }

            fileDialog.OpenResult = new FileDialogResults(xmlPath, true);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(10.0, setting.AxisLogBase.X);
            Assert.AreEqual(2.0, setting.AxisLogBase.Y);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WithXmlFile_ShowsSuccessMessage()
        {
            var xmlPath = Path.Combine(tempDir, "setting.xml");
            var xmlSerializer = new XmlSerializer(typeof(Setting));
            using (var stream = new FileStream(xmlPath, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, new Setting().Copy());
            }
            fileDialog.OpenResult = new FileDialogResults(xmlPath, true);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(1, messageBox.OkMessages.Count);
        }

        // ── LoadSetting — unsupported extension ────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WithUnsupportedExtension_ShowsErrorMessage()
        {
            var badPath = Path.Combine(tempDir, "setting.txt");
            File.WriteAllText(badPath, "anything");
            fileDialog.OpenResult = new FileDialogResults(badPath, true);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(1, messageBox.OkMessages.Count);
            Assert.IsTrue(messageBox.OkMessages[0].message.Contains("extension"));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WithUnsupportedExtension_DoesNotModifySetting()
        {
            var badPath = Path.Combine(tempDir, "setting.txt");
            File.WriteAllText(badPath, "anything");
            fileDialog.OpenResult = new FileDialogResults(badPath, true);
            setting.AxisLogBase = new PointD(3, 3);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(new PointD(3, 3), setting.AxisLogBase);
        }

        // ── LoadSetting — exception path ───────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void LoadSetting_WhenJsonIsInvalid_ShowsErrorMessage()
        {
            var jsonPath = Path.Combine(tempDir, "bad.json");
            File.WriteAllText(jsonPath, "{ NOT_VALID_JSON ~~~");
            fileDialog.OpenResult = new FileDialogResults(jsonPath, true);

            vm.LoadSettingCommand.Execute();

            Assert.AreEqual(1, messageBox.OkMessages.Count);
            Assert.IsTrue(messageBox.OkMessages[0].message.Contains("Error"));
        }

        // ── SaveSetting — cancelled dialog ─────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void SaveSetting_WhenDialogCancelled_WritesNoFile()
        {
            fileDialog.SaveResult = new FileDialogResults(null, false);

            vm.SaveSettingCommand.Execute();

            Assert.AreEqual(0, Directory.GetFiles(tempDir).Length);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void SaveSetting_WhenDialogCancelled_ShowsNoMessage()
        {
            fileDialog.SaveResult = new FileDialogResults(null, false);

            vm.SaveSettingCommand.Execute();

            Assert.AreEqual(0, messageBox.OkMessages.Count);
        }

        // ── SaveSetting — JSON ─────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void SaveSetting_WithJsonPath_WritesJsonFile()
        {
            var jsonPath = Path.Combine(tempDir, "out.json");
            fileDialog.SaveResult = new FileDialogResults(jsonPath, true);
            setting.FilterMax = new Rgba(100, 150, 200, 255);

            vm.SaveSettingCommand.Execute();

            Assert.IsTrue(File.Exists(jsonPath));
            var json = File.ReadAllText(jsonPath);
            var loaded = JsonSerializer.Deserialize<Setting>(json, new JsonSerializerOptions
            {
                Converters = { new RgbaConverter() }
            });
            Assert.AreEqual(100.0, loaded.FilterMax.Red);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void SaveSetting_WithJsonPath_ShowsSuccessMessage()
        {
            var jsonPath = Path.Combine(tempDir, "out.json");
            fileDialog.SaveResult = new FileDialogResults(jsonPath, true);

            vm.SaveSettingCommand.Execute();

            Assert.AreEqual(1, messageBox.OkMessages.Count);
            Assert.IsTrue(messageBox.OkMessages[0].message.Contains("successfully"));
        }

        // ── SaveSetting — XML ──────────────────────────────────────────────

        [TestMethod]
        [TestCategory("Unit")]
        public void SaveSetting_WithXmlPath_WritesXmlFile()
        {
            var xmlPath = Path.Combine(tempDir, "out.xml");
            fileDialog.SaveResult = new FileDialogResults(xmlPath, true);
            setting.AxisLogBase = new PointD(7, 3);

            vm.SaveSettingCommand.Execute();

            Assert.IsTrue(File.Exists(xmlPath));
            var xmlSerializer = new XmlSerializer(typeof(Setting));
            using var stream = new FileStream(xmlPath, FileMode.Open);
            var loaded = (Setting)xmlSerializer.Deserialize(stream);
            Assert.AreEqual(7.0, loaded.AxisLogBase.X);
        }

        // ── SaveSetting — unknown extension falls through to JSON ──────────

        [TestMethod]
        [TestCategory("Unit")]
        public void SaveSetting_WithUnknownExtension_WritesJsonFile()
        {
            // In MainViewModel.SaveSetting, the switch has `default:` before `case ".json":`
            // so an unknown extension falls through to .json behaviour.
            var weirdPath = Path.Combine(tempDir, "out.xyz");
            fileDialog.SaveResult = new FileDialogResults(weirdPath, true);
            setting.FilterMin = new Rgba(5, 10, 15, 255);

            vm.SaveSettingCommand.Execute();

            Assert.IsTrue(File.Exists(weirdPath));
            // Should be valid JSON
            var json = File.ReadAllText(weirdPath);
            var loaded = JsonSerializer.Deserialize<Setting>(json, new JsonSerializerOptions
            {
                Converters = { new RgbaConverter() }
            });
            Assert.AreEqual(5.0, loaded.FilterMin.Red);
        }
    }
}

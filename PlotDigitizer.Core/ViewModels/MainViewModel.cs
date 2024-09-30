using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace PlotDigitizer.Core
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private readonly IFileDialogService fileDialogService;
        private readonly IMessageBoxService messageBoxService;
        private readonly Setting setting;
        private readonly ILogger<MainViewModel> logger;
        #endregion Fields

        #region Properties

        public Model Model { get; }
        public IPageService PageService { get; private set; }
        public RelayCommand LoadSettingCommand { get; private set; }
        public RelayCommand SaveSettingCommand { get; private set; }
        #endregion Properties

        #region Constructors

        public MainViewModel()
        {
            SaveSettingCommand = new RelayCommand(SaveSetting);
            LoadSettingCommand = new RelayCommand(LoadSetting);
        }

        public MainViewModel(
            Model model,
            Setting setting,
            IFileDialogService fileDialogService,
            IMessageBoxService messageBoxService,
            IPageService pageService,
            ILogger<MainViewModel> logger) : this()
        {
            Model = model;
            this.setting = setting;
            this.fileDialogService = fileDialogService;
            this.messageBoxService = messageBoxService;
            PageService = pageService;
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        private void LoadSetting()
        {
            logger?.LogInformation("LoadSetting invoked.");

            var filter = "JSON file (*.json) |*.json|" +
                "XML file (*.xml) |*.xml";
            var filename = "PlotDigitizer Setting";
            var results = fileDialogService.OpenFileDialog(filter, filename);

            if (!results.IsValid) {
                logger?.LogWarning("LoadSetting cancelled by the user or invalid file selection.");
                return;
            }

            var ext = Path.GetExtension(results.FileName).ToLower();
            Setting setting = null;
            logger?.LogDebug($"Loading setting from file: {results.FileName}.");

            try {
                switch (ext) {
                    case ".json":
                        var json = File.ReadAllText(results.FileName);
                        setting = JsonSerializer.Deserialize(json, typeof(Setting), new JsonSerializerOptions
                        {
                            Converters = { new RgbaConverter() }
                        }) as Setting;
                        logger?.LogInformation("Setting successfully loaded from JSON file.");
                        break;

                    case ".xml": {
                            var xmlSerializer = new XmlSerializer(typeof(Setting));
                            using var stream = new FileStream(results.FileName, FileMode.Open);
                            setting = xmlSerializer.Deserialize(stream) as Setting;
                            logger?.LogInformation("Setting successfully loaded from XML file.");
                            break;
                        }
                    default:
                        logger?.LogError("Unsupported file extension: {0}", ext);
                        messageBoxService.Show_OK("Cannot load this file. File extension must be either .json or .xml", "PlotDigitizer");
                        return;
                }

                this.setting.Load(setting);
                messageBoxService.Show_OK("Setting is loaded successfully.", "PlotDigitizer");
                logger?.LogInformation("Setting applied to the application successfully.");
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Error occurred while loading settings from file.");
                messageBoxService.Show_OK("Error occurred while loading the settings.", "PlotDigitizer");
            }
        }

        private void SaveSetting()
        {
            logger?.LogInformation("SaveSetting invoked.");

            var filter = "JSON file (*.json) |*.json|" +
                "XML file (*.xml) |*.xml";
            var filename = "PlotDigitizer Setting";
            var results = fileDialogService.SaveFileDialog(filter, filename);

            if (!results.IsValid) {
                logger?.LogWarning("SaveSetting cancelled by the user or invalid file selection.");
                return;
            }

            logger?.LogDebug($"Saving setting to file: {results.FileName}.");

            try {
                switch (Path.GetExtension(results.FileName).ToLower()) {
                    default:
                    case ".json":
                        var json = JsonSerializer.Serialize(setting, new JsonSerializerOptions
                        {
                            Converters = { new RgbaConverter() }
                        });
                        File.WriteAllText(results.FileName, json);
                        logger?.LogInformation("Setting saved successfully to JSON file.");
                        break;

                    case ".xml": {
                            var xmlSerializer = new XmlSerializer(typeof(Setting));
                            using var stream = new FileStream(results.FileName, FileMode.OpenOrCreate);
                            xmlSerializer.Serialize(stream, setting.Copy());
                            logger?.LogInformation("Setting saved successfully to XML file.");
                            break;
                        }
                }

                messageBoxService.Show_OK("Current setting is saved successfully.", "PlotDigitizer");
                logger?.LogInformation("Save operation completed successfully.");
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Error occurred while saving settings to file.");
                messageBoxService.Show_OK("Error occurred while saving the settings.", "PlotDigitizer");
            }
        }

        #endregion Methods
    }
}

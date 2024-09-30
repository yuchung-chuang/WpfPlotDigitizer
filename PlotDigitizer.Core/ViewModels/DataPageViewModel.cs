using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;

using PropertyChanged;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PlotDigitizer.Core
{
    public class DataPageViewModel : ViewModelBase
    {
        enum ExportResults
        {
            None,
            Canceled,
            Failed,
            Sucessful,
        }

        #region Fields

        private readonly IAwaitTaskService awaitTask;
        private readonly IImageService imageService;
        private readonly IFileDialogService fileDialog;
        private readonly ILogger<DataPageViewModel> logger;
        private readonly IMessageBoxService messageBox;
        private readonly Setting setting;

        #endregion Fields

        #region Properties

        public RelayCommand ExportCommand { get; private set; }

        [OnChangedMethod(nameof(OnIsContinuousChanged))]
        public bool IsContinuous
        {
            get => setting.DataType == DataType.Continuous;
            set => setting.DataType = value ? DataType.Continuous : DataType.Discrete;
        }

        [OnChangedMethod(nameof(OnIsDiscreteChanged))]
        public bool IsDiscrete
        {
            get => setting.DataType == DataType.Discrete;
            set => setting.DataType = value ? DataType.Discrete : DataType.Continuous;
        }

        public bool IsEnabled => Model != null && Model.EdittedImage != null;

        public Model Model { get; }

        public Image<Rgba, byte> Image { get; private set; }

        #endregion Properties

        #region Constructors

        public DataPageViewModel()
        {
            Name = "Data Page";
            ExportCommand = new RelayCommand(Export, CanExport);
        }

        public DataPageViewModel(
            Model model,
            Setting setting,
            IMessageBoxService messageBox,
            IFileDialogService fileDialog,
            IAwaitTaskService awaitTask,
            IImageService imageService,
            ILogger<DataPageViewModel> logger) : this()
        {
            Model = model;
            this.setting = setting;
            this.messageBox = messageBox;
            this.fileDialog = fileDialog;
            this.awaitTask = awaitTask;
            this.imageService = imageService;
            this.logger = logger;

            logger?.LogInformation("DataPageViewModel initialized with Model and services.");
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            logger?.LogInformation($"Entered DataPageViewModel with IsEnabled status: {IsEnabled}");

            if (!IsEnabled) {
                logger?.LogWarning("DataPageViewModel is not enabled. Model or EdittedImage is null.");
                return;
            }

            UpdatePreviewImage();
        }

        private void UpdatePreviewImage()
        {
            logger?.LogInformation($"Updating preview image in {MethodBase.GetCurrentMethod()?.Name}.");

            if (!IsEnabled) {
                logger?.LogWarning("UpdatePreviewImage skipped because DataPageViewModel is not enabled.");
                return;
            }

            Image = Model.EdittedImage.Copy();

            try {
                if (IsDiscrete) {
                    logger?.LogInformation("Drawing discrete markers on the preview image.");
                    imageService.DrawDiscreteMarkers(Image, Model.DataPoints);
                }
                else if (IsContinuous) {
                    logger?.LogInformation("Drawing continuous markers on the preview image.");
                    imageService.DrawContinuousMarkers(Image, Model.DataPoints);
                }
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Error while drawing markers on the preview image.");
            }

            RaisePropertyChanged(nameof(Image));
            ExportCommand.RaiseCanExecuteChanged();
            logger?.LogInformation("UpdatePreviewImage completed successfully.");
        }

        private bool CanExport()
        {
            var canExport = Model?.Data?.Count() > 0;
            logger?.LogInformation($"CanExport evaluated to {canExport}.");
            return canExport;
        }

        private void Export()
        {
            logger?.LogInformation("Export operation started.");

            var filter = "Comma-separated values file (*.csv) |*.csv|" +
                         "Text file (*.txt) |*.txt|" +
                         "Any (*.*) |*.*";
            var result = fileDialog.SaveFileDialog(filter, "output");

            if (!result.IsValid) {
                logger?.LogInformation("Export canceled by user. SaveFileDialog returned invalid result.");
                return;
            }

            logger?.LogInformation($"User selected file: {result.FileName}");

            TrySave(result.FileName);

            async void TrySave(string fileName)
            {
                logger?.LogInformation($"Starting async export to {fileName}.");

                var exportResult = await awaitTask.RunAsync((token) =>
                {
                    try {
                        return Path.GetExtension(fileName).ToLower() switch
                        {
                            ".txt" => SaveAsTXT(token),
                            ".csv" => SaveAsCSV(token),
                            _ => throw new FormatException("Output file format is not recognized. Please use either .csv or .txt as file extension."),
                        };
                    }
                    catch (Exception ex) {
                        logger?.LogError(ex, "Error during file export.");
                        return ExportResults.Failed;
                    }
                });

                logger?.LogInformation($"Export completed with result: {exportResult}");

                switch (exportResult) {
                    case ExportResults.Sucessful:
                        messageBox.Show_OK("The data has been exported successfully.", "Notification");
                        logger?.LogInformation("Data export successful.");
                        break;

                    case ExportResults.Failed:
                        logger?.LogWarning("Data export failed.");
                        var response = messageBox.Show_Warning_OkCancel("Something went wrong... try again?", "Error");
                        if (response) {
                            logger?.LogInformation("User chose to retry export.");
                            TrySave(fileName);
                        }
                        break;

                    case ExportResults.Canceled:
                        messageBox.Show_OK("Export operation has been cancelled.", "Notification");
                        logger?.LogInformation("Export operation was canceled by the user.");
                        break;

                    case ExportResults.None:
                    default:
                        break;
                }
                logger?.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");

                ExportResults SaveAsCSV(CancellationToken token) => SaveText(",", token);
                ExportResults SaveAsTXT(CancellationToken token) => SaveText("\t", token);
                ExportResults SaveText(string separator, CancellationToken token)
                {
                    try {
                        logger?.LogInformation($"Saving data as text using separator: {separator}");

                        var content = new StringBuilder();
                        var xlabel = !string.IsNullOrWhiteSpace(setting.AxisTitle.XLabel) ? setting.AxisTitle.XLabel : "X";
                        var ylabel = !string.IsNullOrWhiteSpace(setting.AxisTitle.YLabel) ? setting.AxisTitle.YLabel : "Y";
                        content.AppendLine(xlabel + separator + ylabel);

                        foreach (var point in Model.Data) {
                            content.AppendLine($"{point.X}{separator}{point.Y}");
                            if (token.IsCancellationRequested) {
                                logger?.LogInformation("Export operation canceled during text saving.");
                                return ExportResults.Canceled;
                            }
                        }

                        using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                        using (var sw = new StreamWriter(fs)) {
                            sw.Write(content.ToString());
                        }

                        logger?.LogInformation($"Data saved successfully to {fileName}.");
                        return ExportResults.Sucessful;
                    }
                    catch (Exception ex) {
                        logger?.LogError(ex, $"Error while saving data to file: {fileName}.");
                        return ExportResults.Failed;
                    }
                }
            }
        }

        private void OnIsContinuousChanged()
        {
            logger?.LogInformation("IsContinuous property changed.");
            UpdatePreviewImage();
        }

        private void OnIsDiscreteChanged()
        {
            logger?.LogInformation("IsDiscrete property changed.");
            UpdatePreviewImage();
        }

        #endregion Methods
    }
}

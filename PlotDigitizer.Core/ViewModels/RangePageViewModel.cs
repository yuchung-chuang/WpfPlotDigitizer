using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Input;

namespace PlotDigitizer.Core
{
    public class RangePageViewModel : ViewModelBase
    {
        #region Fields

        private readonly Setting setting;
        private readonly IImageService imageService;
        private readonly IOcrService numericalOcrService;
        private readonly IOcrService textOcrService;
        private readonly ILogger<RangePageViewModel> logger;

        #endregion Fields

        #region Properties

        public double XMax { get; set; } = double.NaN;
        public double XMin { get; set; } = double.NaN;
        public double YMax { get; set; } = double.NaN;
        public double YMin { get; set; } = double.NaN;

        public RectangleD AxisLimit
        {
            get => new(XMin, YMin, XMax - XMin, YMax - YMin);
            set {
                XMax = value.Right;
                XMin = value.Left;
                YMax = value.Bottom;
                YMin = value.Top;
                logger?.LogInformation($"AxisLimit set to: {AxisLimit}");
            }
        }

        public double XLog { get; set; } = double.NaN;
        public double YLog { get; set; } = double.NaN;
        public PointD AxisLogBase
        {
            get => new(XLog, YLog);
            set {
                XLog = value.X;
                YLog = value.Y;
                logger?.LogInformation($"AxisLogBase set to: {AxisLogBase}");
            }
        }

        public string XLabel { get; set; }
        public string YLabel { get; set; }

        public AxisTitle AxisTitle
        {
            get => new(XLabel, YLabel);
            set {
                XLabel = value.XLabel;
                YLabel = value.YLabel;
                logger?.LogInformation($"AxisTitle set to: XLabel: {XLabel}, YLabel: {YLabel}");
            }
        }

        public RectangleD XMaxTextBox { get; set; }
        public RectangleD XMinTextBox { get; set; }
        public RectangleD YMaxTextBox { get; set; }
        public RectangleD YMinTextBox { get; set; }
        public RectangleD XLabelTextBox { get; set; }
        public RectangleD YLabelTextBox { get; set; }

        public bool IsEnabled => Model != null && Model.InputImage != null;
        public Model Model { get; }
        public Image<Rgba, byte> Image => !IsEnabled ? null : Model.InputImage;

        public ICommand OcrCommand { get; }

        #endregion Properties

        #region Constructors

        public RangePageViewModel()
        {
            Name = "Axis Range Page";
            OcrCommand = new RelayCommand(Ocr);
        }

        public RangePageViewModel(Model model,
            Setting setting,
            IImageService imageService,
            [FromKeyedServices("Numerical")] IOcrService numericalOcrService,
            [FromKeyedServices("Text")] IOcrService textOcrService,
            ILogger<RangePageViewModel> logger) : this()
        {
            Model = model;
            this.setting = setting;
            this.imageService = imageService;
            this.numericalOcrService = numericalOcrService;
            this.textOcrService = textOcrService;
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            logger?.LogInformation($"Entered RangePageViewModel with IsEnabled status: {IsEnabled}");

            if (!IsEnabled) {
                logger?.LogWarning($"RangePageViewModel is not enabled because Model or InputImage is null.");
                return;
            }

            if (setting.AxisTextBox == default) {
                logger?.LogInformation("No stored AxisTextBox found in settings. Detecting axis text boxes.");

                var textBox = imageService.GetAxisTextBox(Image, setting.AxisLocation);

                XMaxTextBox = textBox.XMax != default ? new RectangleD(textBox.XMax) : new RectangleD();
                XMinTextBox = textBox.XMin != default ? new RectangleD(textBox.XMin) : new RectangleD();
                YMaxTextBox = textBox.YMax != default ? new RectangleD(textBox.YMax) : new RectangleD();
                YMinTextBox = textBox.YMin != default ? new RectangleD(textBox.YMin) : new RectangleD();
                XLabelTextBox = textBox.XLabel != default ? new RectangleD(textBox.XLabel) : new RectangleD();
                YLabelTextBox = textBox.YLabel != default ? new RectangleD(textBox.YLabel) : new RectangleD();

                logger?.LogInformation($"Detected axis text boxes: XMaxTextBox: {XMaxTextBox}, XMinTextBox: {XMinTextBox}, YMaxTextBox: {YMaxTextBox}, YMinTextBox: {YMinTextBox}, XLabelTextBox: {XLabelTextBox}, YLabelTextBox: {YLabelTextBox}");

                Ocr();
            }
            else {
                XMaxTextBox = setting.AxisTextBox.XMax;
                XMinTextBox = setting.AxisTextBox.XMin;
                YMaxTextBox = setting.AxisTextBox.YMax;
                YMinTextBox = setting.AxisTextBox.YMin;
                XLabelTextBox = setting.AxisTextBox.XLabel;
                YLabelTextBox = setting.AxisTextBox.YLabel;
                AxisLimit = setting.AxisLimit;
                AxisTitle = setting.AxisTitle;

                logger?.LogInformation("Loaded AxisTextBox and other settings from stored values.");
            }

            AxisLogBase = setting.AxisLogBase;
        }

        public override void Leave()
        {
            base.Leave();
            logger?.LogInformation($"Leaving RangePageViewModel with IsEnabled status: {IsEnabled}");

            if (!IsEnabled) {
                logger?.LogWarning($"RangePageViewModel is not enabled because Model or InputImage is null.");
                return;
            }

            setting.AxisTextBox = new AxisLimitTextBoxD
            {
                XMax = XMaxTextBox,
                XMin = XMinTextBox,
                YMax = YMaxTextBox,
                YMin = YMinTextBox,
                XLabel = XLabelTextBox,
                YLabel = YLabelTextBox
            };

            setting.AxisTitle = AxisTitle;
            setting.AxisLimit = AxisLimit;
            setting.AxisLogBase = AxisLogBase;

            logger?.LogInformation("Stored AxisTextBox, AxisTitle, AxisLimit, and AxisLogBase to settings.");
        }

        private void Ocr()
        {
            logger?.LogInformation("Starting OCR on axis text boxes.");

            try {
                var roi = imageService.CropImage(Image, XMaxTextBox.ToRectangle());
                var xMaxText = numericalOcrService.Ocr(roi);
                if (double.TryParse(xMaxText, out var xMax)) {
                    XMax = xMax;
                    logger?.LogInformation($"OCR successful: XMax = {XMax}");
                }

                roi = imageService.CropImage(Image, XMinTextBox.ToRectangle());
                var xMinText = numericalOcrService.Ocr(roi);
                if (double.TryParse(xMinText, out var xMin)) {
                    XMin = xMin;
                    logger?.LogInformation($"OCR successful: XMin = {XMin}");
                }

                roi = imageService.CropImage(Image, YMaxTextBox.ToRectangle());
                var yMaxText = numericalOcrService.Ocr(roi);
                if (double.TryParse(yMaxText, out var yMax)) {
                    YMax = yMax;
                    logger?.LogInformation($"OCR successful: YMax = {YMax}");
                }

                roi = imageService.CropImage(Image, YMinTextBox.ToRectangle());
                var yMinText = numericalOcrService.Ocr(roi);
                if (double.TryParse(yMinText, out var yMin)) {
                    YMin = yMin;
                    logger?.LogInformation($"OCR successful: YMin = {YMin}");
                }

                roi = imageService.CropImage(Image, XLabelTextBox.ToRectangle());
                XLabel = textOcrService.Ocr(roi).Trim();
                logger?.LogInformation($"OCR successful: XLabel = {XLabel}");

                roi = imageService.CropImage(Image, YLabelTextBox.ToRectangle());
                roi = imageService.RotateImage(roi, 90);
                YLabel = textOcrService.Ocr(roi).Trim();
                logger?.LogInformation($"OCR successful: YLabel = {YLabel}");
            }
            catch (Exception ex) {
                logger?.LogError($"An error occurred during the OCR process: {ex.Message}");
            }
        }

        #endregion Methods
    }
}

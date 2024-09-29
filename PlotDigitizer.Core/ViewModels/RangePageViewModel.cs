using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.ComponentModel;
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
            [FromKeyedServices("Text")] IOcrService textOcrService) : this()
        {
            Model = model;
            this.setting = setting;
            this.imageService = imageService;
            this.numericalOcrService = numericalOcrService;
            this.textOcrService = textOcrService;
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            if (!IsEnabled) {
                return;
            }
            if (setting.AxisTextBox == default) {
                var textBox = imageService.GetAxisTextBox(Image, setting.AxisLocation);
                if (textBox.XMax != default) {
                    XMaxTextBox = new RectangleD(textBox.XMax);
                }
                if (textBox.XMin != default) {
                    XMinTextBox = new RectangleD(textBox.XMin);
                }
                if (textBox.YMax != default) {
                    YMaxTextBox = new RectangleD(textBox.YMax);
                }
                if (textBox.YMin != default) {
                    YMinTextBox = new RectangleD(textBox.YMin);
                }
                if (textBox.XLabel != default) {
                    XLabelTextBox = new RectangleD(textBox.XLabel);
                }
                if (textBox.YMin != default) {
                    YLabelTextBox = new RectangleD(textBox.YLabel);
                }
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
            }
            AxisLogBase = setting.AxisLogBase;
        }

        public override void Leave()
        {
            base.Leave();
            if (!IsEnabled) {
                return;
            }
            setting.AxisTextBox = new AxisLimitTextBoxD
            {
                XMax = XMaxTextBox,
                XMin = XMinTextBox,
                YMax = YMaxTextBox,
                YMin = YMinTextBox,
                XLabel = XLabelTextBox,
                YLabel = YLabelTextBox,
            };
            setting.AxisTitle = AxisTitle;
            setting.AxisLimit = AxisLimit;
            setting.AxisLogBase = AxisLogBase;
        }

        private void Ocr()
        {
            var roi = imageService.CropImage(Image, XMaxTextBox.ToRectangle());
            var xMaxText = numericalOcrService.Ocr(roi);
            if (double.TryParse(xMaxText, out var xMax))
                XMax = xMax;

            roi = imageService.CropImage(Image, XMinTextBox.ToRectangle());
            var xMinText = numericalOcrService.Ocr(roi);
            if (double.TryParse(xMinText, out var xMin))
                XMin = xMin;

            roi = imageService.CropImage(Image, YMaxTextBox.ToRectangle());
            var yMaxText = numericalOcrService.Ocr(roi);
            if (double.TryParse(yMaxText, out var yMax))
                YMax = yMax;

            roi = imageService.CropImage(Image, YMinTextBox.ToRectangle());
            var yMinText = numericalOcrService.Ocr(roi);
            if (double.TryParse(yMinText, out var yMin))
                YMin = yMin;

            roi = imageService.CropImage(Image, XLabelTextBox.ToRectangle());
            XLabel = textOcrService.Ocr(roi).Trim();

            roi = imageService.CropImage(Image, YLabelTextBox.ToRectangle());
            roi = imageService.RotateImage(roi, 90);
            YLabel = textOcrService.Ocr(roi).Trim();
        }


        #endregion Methods
    }
}
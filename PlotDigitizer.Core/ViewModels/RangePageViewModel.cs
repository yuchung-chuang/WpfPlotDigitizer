using Emgu.CV;
using Emgu.CV.Structure;
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
        private readonly IOcrService ocrService;

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
        public double XMaxBoxLeft { get; set; }
        public double XMaxBoxTop { get; set; }
        public double XMaxBoxWidth { get; set; }
        public double XMaxBoxHeight { get; set; }
        public RectangleD XMaxTextBox
        {
            get => new(XMaxBoxLeft, XMaxBoxTop, XMaxBoxWidth, XMaxBoxHeight);
            set {
                XMaxBoxLeft = value.Left;
                XMaxBoxTop = value.Top;
                XMaxBoxWidth = value.Width;
                XMaxBoxHeight = value.Height;
            }
        }

        public double XMinBoxLeft { get; set; }
        public double XMinBoxTop { get; set; }
        public double XMinBoxWidth { get; set; }
        public double XMinBoxHeight { get; set; }
        public RectangleD XMinTextBox
        {
            get => new(XMinBoxLeft, XMinBoxTop, XMinBoxWidth, XMinBoxHeight);
            set {
                XMinBoxLeft = value.Left;
                XMinBoxTop = value.Top;
                XMinBoxWidth = value.Width;
                XMinBoxHeight = value.Height;
            }
        }

        public double YMaxBoxLeft { get; set; }
        public double YMaxBoxTop { get; set; }
        public double YMaxBoxWidth { get; set; }
        public double YMaxBoxHeight { get; set; }
        public RectangleD YMaxTextBox
        {
            get => new(YMaxBoxLeft, YMaxBoxTop, YMaxBoxWidth, YMaxBoxHeight);
            set {
                YMaxBoxLeft = value.Left;
                YMaxBoxTop = value.Top;
                YMaxBoxWidth = value.Width;
                YMaxBoxHeight = value.Height;
            }
        }

        public double YMinBoxLeft { get; set; }
        public double YMinBoxTop { get; set; }
        public double YMinBoxWidth { get; set; }
        public double YMinBoxHeight { get; set; }
        public RectangleD YMinTextBox
        {
            get => new(YMinBoxLeft, YMinBoxTop, YMinBoxWidth, YMinBoxHeight);
            set {
                YMinBoxLeft = value.Left;
                YMinBoxTop = value.Top;
                YMinBoxWidth = value.Width;
                YMinBoxHeight = value.Height;
            }
        }

        public bool IsEnabled => Model != null && Model.InputImage != null;
        public Model Model { get; }
        public Image<Rgba, byte> Image => !IsEnabled ? null : Model.InputImage;

        public ICommand OcrCommand { get; }

        #endregion Properties

        #region Constructors

        public RangePageViewModel()
        {
            Name = "Axis Range Page";
            OcrCommand = new RelayCommand(OCR);
        }

        public RangePageViewModel(Model model,
            Setting setting,
            IImageService imageService,
            IOcrService ocrService) : this()
        {
            Model = model;
            this.setting = setting;
            this.imageService = imageService;
            this.ocrService = ocrService;
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            if (!IsEnabled) {
                return;
            }
            if (setting.AxisLimitTextBox is null) {
                var textBox = imageService.GetAxisLimitTextBoxes(Image, setting.AxisLocation);
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
                OCR();
            } else { 
                XMaxTextBox = setting.AxisLimitTextBox.XMax;
                XMinTextBox = setting.AxisLimitTextBox.XMin;
                YMaxTextBox = setting.AxisLimitTextBox.YMax;
                YMinTextBox = setting.AxisLimitTextBox.YMin;
                AxisLimit = setting.AxisLimit;
            }
            AxisLogBase = setting.AxisLogBase;
        }

        public override void Leave()
        {
            base.Leave();
            if (!IsEnabled) {
                return;
            }
            setting.AxisLimitTextBox = new AxisLimitTextBoxD
            {
                XMax = XMaxTextBox,
                XMin = XMinTextBox,
                YMax = YMaxTextBox,
                YMin = YMinTextBox,
            };
            setting.AxisLimit = AxisLimit;
            setting.AxisLogBase = AxisLogBase;
        }
        
        private void OCR()
        {
            XMax = ocrService.OcrDouble(Image, XMaxTextBox.ToRectangle());
            XMin = ocrService.OcrDouble(Image, XMinTextBox.ToRectangle());
            YMax = ocrService.OcrDouble(Image, YMaxTextBox.ToRectangle());
            YMin = ocrService.OcrDouble(Image, YMinTextBox.ToRectangle());
        }


        #endregion Methods
    }
}
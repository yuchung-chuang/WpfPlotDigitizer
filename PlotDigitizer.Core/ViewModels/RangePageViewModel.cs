using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
    public class RangePageViewModel : PageViewModelBase
    {
        #region Fields

        private readonly Setting setting;
        private readonly IImageService imageService;
        private readonly IOcrService ocrService;

        #endregion Fields

        #region Properties
        public double BoxLeft { get; set; }
        public double BoxTop { get; set; }
        public double BoxWidth { get; set; }
        public double BoxHeight { get; set; }

        public RectangleD XMaxTextBox { 
            get => new(BoxLeft, BoxTop, BoxWidth, BoxHeight); 
            set { 
                BoxLeft = value.Left;
                BoxTop = value.Top;
                BoxWidth = value.Width;
                BoxHeight = value.Height;
            } 
        }

        public RectangleD AxisLimit
        {
            get => new(AxLimXMin, AxLimYMin, AxLimXMax - AxLimXMin, AxLimYMax - AxLimYMin);
            set {
                AxLimXMax = value.Right;
                AxLimXMin = value.Left;
                AxLimYMax = value.Bottom;
                AxLimYMin = value.Top;
            }
        }

        public PointD AxisLogBase
        {
            get => new(AxLimXLog, AxLimYLog);
            set {
                AxLimXLog = value.X;
                AxLimYLog = value.Y;
            }
        }

        public double AxLimXLog { get; set; } = double.NaN;
        public double AxLimXMax { get; set; } = double.NaN;
        public double AxLimXMin { get; set; } = double.NaN;
        public double AxLimYLog { get; set; } = double.NaN;
        public double AxLimYMax { get; set; } = double.NaN;
        public double AxLimYMin { get; set; } = double.NaN;
        public bool IsEnabled => Model != null && Model.InputImage != null;
        public Model Model { get; }
        public Image<Rgba, byte> Image => !IsEnabled ? null : Model.InputImage;

        #endregion Properties

        #region Constructors

        public RangePageViewModel()
        {
            Name = "AxisLimitPage";
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
            model.PropertyChanged += Model_PropertyChanged;
            setting.PropertyChanged += Setting_PropertyChanged;
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            if (!IsEnabled) {
                return;
            }
            if (setting.AxisLimit == default) {
                var (xMaxTextBox, xMinTextBox, yMaxTextBox, yMinTextBox) = imageService.GetAxisLimitTextBoxes(Image, setting.AxisLocation);
                if (xMaxTextBox != default) {
                    XMaxTextBox = new RectangleD(xMaxTextBox);
                }
                var xMin = ocrService.OcrDouble(Image, xMinTextBox);
                var xMax = ocrService.OcrDouble(Image, xMaxTextBox);
                var yMin = ocrService.OcrDouble(Image, yMinTextBox);
                var yMax = ocrService.OcrDouble(Image, yMaxTextBox);
                AxisLimit = new RectangleD(xMin, yMin, xMax - xMin, yMax - yMin);
            }
            else {
                AxisLimit = setting.AxisLimit;
            }
            AxisLogBase = setting.AxisLogBase;
            OnPropertyChanged(nameof(AxLimXMin));
            OnPropertyChanged(nameof(AxLimXLog));
            OnPropertyChanged(nameof(AxLimXMax));
            OnPropertyChanged(nameof(AxLimYMin));
            OnPropertyChanged(nameof(AxLimYLog));
            OnPropertyChanged(nameof(AxLimYMax));
        }

        public override void Leave()
        {
            base.Leave();
            if (!IsEnabled) {
                return;
            }
            setting.AxisLimit = AxisLimit;
            setting.AxisLogBase = AxisLogBase;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Core.Model.InputImage)) {
                base.OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not Setting setting) {
                return;
            }
            if (e.PropertyName == nameof(Setting.AxisLimit)) {
                AxisLimit = setting.AxisLimit;
            }
            if (e.PropertyName == nameof(Setting.AxisLogBase)) {
                AxisLogBase = setting.AxisLogBase;
            }
        }

        #endregion Methods
    }
}
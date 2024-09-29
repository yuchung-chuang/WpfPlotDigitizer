using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
    public class AxisPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly Setting setting;
        private readonly IImageService imageService;

        #endregion Fields

        #region Properties
        public RectangleD AxisLocation { get; set; }
        public RectangleD AxisRelative =>
            Image is null ?
            new() :
            new(AxisLocation.Left / Image.Width,
                AxisLocation.Top / Image.Height,
                AxisLocation.Width / Image.Width,
                AxisLocation.Height / Image.Height);
        public RelayCommand GetAxisCommand { get; set; }
        public bool IsEnabled => Model != null && Model.InputImage != null;
        public Model Model { get; }
        public Image<Rgba, byte> Image => !IsEnabled ? null : Model.InputImage;

        #endregion Properties

        #region Constructors

        public AxisPageViewModel()
        {
            Name = "Axis Page";
            GetAxisCommand = new RelayCommand(GetAxis, CanGetAxis);
        }

        public AxisPageViewModel(Model model,
            Setting setting,
            IImageService imageService) : this()
        {
            Model = model;
            this.setting = setting;
            this.imageService = imageService;
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            if (!IsEnabled) {
                return;
            }
            if (setting.AxisLocation == default) {
                if (GetAxisCommand.CanExecute()) {
                    GetAxisCommand.Execute();
                }
            }
            else {
                AxisLocation = setting.AxisLocation;
            }
        }

        public override void Leave()
        {
            base.Leave();
            if (!IsEnabled) {
                return;
            }
            setting.AxisLocation = AxisLocation;
        }

        private bool CanGetAxis() => IsEnabled;

        private void GetAxis()
        {
            var image = Model.InputImage;
            AxisLocation = imageService.GetAxisLocation(image) ??
                new RectangleD(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
        }

        #endregion Methods
    }
}
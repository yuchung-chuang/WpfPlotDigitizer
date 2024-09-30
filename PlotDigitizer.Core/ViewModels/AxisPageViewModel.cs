using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using System;

namespace PlotDigitizer.Core
{
    public class AxisPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly Setting setting;
        private readonly IImageService imageService;
        private readonly ILogger<AxisPageViewModel> logger;

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
            IImageService imageService,
            ILogger<AxisPageViewModel> logger) : this()
        {
            Model = model;
            this.setting = setting;
            this.imageService = imageService;
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        public override void Enter()
        {
            base.Enter();
            logger?.LogInformation($"Entered AxisPageViewModel with IsEnabled status: {IsEnabled}");

            if (!IsEnabled) {
                logger?.LogWarning($"Model or InputImage is null. AxisPageViewModel is not enabled.");
                return;
            }

            if (setting.AxisLocation == default) {
                logger?.LogInformation($"No stored axis location found in settings. Attempting to auto-detect axis location.");
                if (GetAxisCommand.CanExecute()) {
                    GetAxisCommand.Execute();
                }
            }
            else {
                AxisLocation = setting.AxisLocation;
                logger?.LogInformation($"Loaded stored axis location from settings: {AxisLocation}");
            }
        }

        public override void Leave()
        {
            base.Leave();
            logger?.LogInformation($"Leaving AxisPageViewModel with IsEnabled status: {IsEnabled}");

            if (!IsEnabled) {
                logger?.LogWarning($"Model or InputImage is null. AxisPageViewModel is not enabled.");
                return;
            }

            setting.AxisLocation = AxisLocation;
            logger?.LogInformation($"Stored current axis location to settings: {AxisLocation}");
        }

        private bool CanGetAxis()
        {
            logger?.LogDebug($"CanGetAxis evaluated. IsEnabled status: {IsEnabled}");
            return IsEnabled;
        }

        private void GetAxis()
        {
            if (!IsEnabled) {
                logger?.LogWarning($"Attempted to get axis location when AxisPageViewModel is not enabled.");
                return;
            }

            logger?.LogInformation($"Starting axis location detection for the current image.");

            var image = Model.InputImage;

            try {
                AxisLocation = imageService.GetAxisLocation(image) ??
                    new RectangleD(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);

                logger?.LogInformation($"Axis location detected successfully: {AxisLocation}");
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Failed to detect axis location.");
                AxisLocation = new RectangleD(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
                logger?.LogWarning($"Using default axis location: {AxisLocation}");
            }
        }

        #endregion Methods
    }
}

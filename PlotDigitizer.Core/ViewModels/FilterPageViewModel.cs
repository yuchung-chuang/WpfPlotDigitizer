using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using PropertyChanged;
using System;

namespace PlotDigitizer.Core
{
    public class FilterPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly Setting setting;
        private readonly IImageService imageService;
        private readonly ILogger<FilterPageViewModel> logger;

        #endregion Fields

        #region Properties

        public Image<Rgba, byte> CroppedImage => Model?.CroppedImage;

        public Rgba FilterMax
        {
            get => new(MaxR, MaxG, MaxB, byte.MaxValue);
            set {
                MaxR = value.Red;
                MaxG = value.Green;
                MaxB = value.Blue;
                logger?.LogInformation("FilterMax set to: R={Red}, G={Green}, B={Blue}", value.Red, value.Green, value.Blue);
            }
        }

        public Rgba FilterMin
        {
            get => new(MinR, MinG, MinB, byte.MaxValue);
            set {
                MinR = value.Red;
                MinG = value.Green;
                MinB = value.Blue;
                logger?.LogInformation("FilterMin set to: R={Red}, G={Green}, B={Blue}", value.Red, value.Green, value.Blue);
            }
        }

        public Image<Rgba, byte> Image { get; private set; }

        public bool IsEnabled => Model != null && Model.CroppedImage != null;

        [OnChangedMethod(nameof(OnMaxBChanged))]
        public double MaxB { get; set; } = byte.MaxValue - 1;

        [OnChangedMethod(nameof(OnMaxGChanged))]
        public double MaxG { get; set; } = byte.MaxValue - 1;

        [OnChangedMethod(nameof(OnMaxRChanged))]
        public double MaxR { get; set; } = byte.MaxValue - 1;

        [OnChangedMethod(nameof(OnMinBChanged))]
        public double MinB { get; set; } = 0;

        [OnChangedMethod(nameof(OnMinGChanged))]
        public double MinG { get; set; } = 0;

        [OnChangedMethod(nameof(OnMinRChanged))]
        public double MinR { get; set; } = 0;

        public Model Model { get; }

        #endregion Properties

        #region Constructors

        public FilterPageViewModel()
        {
            Name = "Filter Page";
        }

        public FilterPageViewModel(Model model,
            Setting setting,
            IImageService imageService,
            ILogger<FilterPageViewModel> logger) : this()
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
            logger?.LogInformation("Entered FilterPageViewModel with IsEnabled status: {IsEnabled}", IsEnabled);

            if (!IsEnabled) {
                logger?.LogWarning("FilterPageViewModel is not enabled because Model or CroppedImage is null.");
                return;
            }

            FilterMin = setting.FilterMin;
            FilterMax = setting.FilterMax;
            logger?.LogInformation("Loaded FilterMin and FilterMax from settings: Min={FilterMin}, Max={FilterMax}", FilterMin, FilterMax);

            FilterImage();
        }

        public void FilterImage()
        {
            if (!IsEnabled) {
                logger?.LogWarning("FilterImage operation aborted because FilterPageViewModel is not enabled.");
                return;
            }

            try {
                logger?.LogInformation("Starting image filtering operation.");
                Image = imageService.FilterRGB(CroppedImage, FilterMin, FilterMax);
                logger?.LogInformation("Image filtering completed successfully.");
            }
            catch (Exception ex) {
                logger?.LogError(ex, "An error occurred during the image filtering process.");
            }
        }

        public override void Leave()
        {
            base.Leave();
            logger?.LogInformation("Leaving FilterPageViewModel with IsEnabled status: {IsEnabled}", IsEnabled);

            if (!IsEnabled) {
                logger?.LogWarning("FilterPageViewModel is not enabled because Model or CroppedImage is null.");
                return;
            }

            setting.FilterMin = FilterMin;
            setting.FilterMax = FilterMax;
            logger?.LogInformation("Stored FilterMin and FilterMax to settings: Min={FilterMin}, Max={FilterMax}", FilterMin, FilterMax);
        }

        private void OnMaxBChanged()
        {
            logger?.LogInformation("MaxB changed to {MaxB}", MaxB);
            FilterImage();
        }

        private void OnMaxGChanged()
        {
            logger?.LogInformation("MaxG changed to {MaxG}", MaxG);
            FilterImage();
        }

        private void OnMaxRChanged()
        {
            logger?.LogInformation("MaxR changed to {MaxR}", MaxR);
            FilterImage();
        }

        private void OnMinBChanged()
        {
            logger?.LogInformation("MinB changed to {MinB}", MinB);
            FilterImage();
        }

        private void OnMinGChanged()
        {
            logger?.LogInformation("MinG changed to {MinG}", MinG);
            FilterImage();
        }

        private void OnMinRChanged()
        {
            logger?.LogInformation("MinR changed to {MinR}", MinR);
            FilterImage();
        }

        #endregion Methods
    }
}

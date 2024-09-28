using Emgu.CV;
using Emgu.CV.Structure;

using PropertyChanged;

using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class FilterPageViewModel : PageViewModelBase
	{
		#region Fields

		private readonly Setting setting;
		private readonly IImageService imageService;

		#endregion Fields

		#region Properties

		public Image<Rgba, byte> CroppedImage => Model?.CroppedImage;

		public Rgba FilterMax
		{
			get => new(MaxR, MaxG, MaxB, byte.MaxValue);
			set
			{
				MaxR = value.Red;
				MaxG = value.Green;
				MaxB = value.Blue;
			}
		}

		public Rgba FilterMin
		{
			get => new(MinR, MinG, MinB, byte.MaxValue);
			set
			{
				MinR = value.Red;
				MinG = value.Green;
				MinB = value.Blue;
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
			IImageService imageService) : this()
		{
			Model = model;
			this.setting = setting;
			this.imageService = imageService;
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
			FilterMin = setting.FilterMin;
			FilterMax = setting.FilterMax;
			FilterImage();
		}

		public void FilterImage()
		{
			if (!IsEnabled) {
				return;
			}
			Image = imageService.FilterRGB(CroppedImage, FilterMin, FilterMax);
		}

		public override void Leave()
		{
			base.Leave();
			if (!IsEnabled) {
				return;
			}
			setting.FilterMin = FilterMin;
			setting.FilterMax = FilterMax;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.CroppedImage)) {
				base.OnPropertyChanged(nameof(IsEnabled));
			}
		}

		private void OnMaxBChanged() => FilterImage();

		private void OnMaxGChanged() => FilterImage();

		private void OnMaxRChanged() => FilterImage();

		private void OnMinBChanged() => FilterImage();

		private void OnMinGChanged() => FilterImage();

		private void OnMinRChanged() => FilterImage();

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is not Setting setting) {
				return;
			}
			if (e.PropertyName == nameof(Setting.FilterMin)) {
				FilterMin = setting.FilterMin;
			} else if (e.PropertyName == nameof(Setting.FilterMax)) {
				FilterMax = setting.FilterMax;
			}
		}

		#endregion Methods
	}
}
using Emgu.CV;
using Emgu.CV.Structure;
using PropertyChanged;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class FilterPageViewModel : PageViewModelBase
	{
		private readonly Setting setting;

		[OnChangedMethod(nameof(OnMinRChanged))]
		public double MinR { get; set; } = 0;
		[OnChangedMethod(nameof(OnMaxRChanged))]
		public double MaxR { get; set; } = byte.MaxValue - 1;
		[OnChangedMethod(nameof(OnMinGChanged))]
		public double MinG { get; set; } = 0;
		[OnChangedMethod(nameof(OnMaxGChanged))]
		public double MaxG { get; set; } = byte.MaxValue - 1;
		[OnChangedMethod(nameof(OnMinBChanged))]
		public double MinB { get; set; } = 0;
		[OnChangedMethod(nameof(OnMaxBChanged))]
		public double MaxB { get; set; } = byte.MaxValue - 1;

		public Rgba FilterMin
		{
			get => new Rgba(MinR, MinG, MinB, byte.MaxValue);
			set
			{
				MinR = value.Red;
				MinG = value.Green;
				MinB = value.Blue;
			}
		}

		public Rgba FilterMax
		{
			get => new Rgba(MaxR, MaxG, MaxB, byte.MaxValue); 
			set
			{
				MaxR = value.Red;
				MaxG = value.Green;
				MaxB = value.Blue;
			}
		}
		public bool IsEnabled => Model != null && Model.CroppedImage != null;

		public Image<Rgba, byte> Image { get; private set; }
		public Image<Rgba, byte> CroppedImage => Model?.CroppedImage;
		public Model Model { get; }
		public FilterPageViewModel()
		{
			Name = "FilterPage";
		}

		public FilterPageViewModel(Model model, Setting setting) : this()
		{
			Model = model;
			this.setting = setting;
			model.PropertyChanged += Model_PropertyChanged;
			setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.CroppedImage)) {
				base.OnPropertyChanged(nameof(IsEnabled));
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(Setting.FilterMin)) {
				FilterMin = setting.FilterMin;
			} else if (e.PropertyName == nameof(Setting.FilterMax)) {
				FilterMax = setting.FilterMax;
			}
		}

		public void FilterImage()
		{
			if (!IsEnabled) {
				return;
			}
			Image = Methods.FilterRGB(CroppedImage, FilterMin, FilterMax);
		}

		private void OnMinRChanged() => FilterImage();
		private void OnMinGChanged() => FilterImage();
		private void OnMinBChanged() => FilterImage();
		private void OnMaxRChanged() => FilterImage();
		private void OnMaxGChanged() => FilterImage();
		private void OnMaxBChanged() => FilterImage();

		public override void Enter()
		{
			base.Enter();
			if (IsEnabled) {
				FilterImage();
			}
		}

		public override void Leave()
		{
			base.Leave();
			if (IsEnabled) {
				setting.FilterMin = FilterMin;
				setting.FilterMax = FilterMax;
			}
		}
	}
}

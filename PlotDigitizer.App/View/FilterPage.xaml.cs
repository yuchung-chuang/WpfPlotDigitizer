using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public partial class FilterPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Enabled => model != null && model.CroppedImage != null;

		[OnChangedMethod(nameof(OnMinRChanged))]
		public double MinR { get; set; }
		[OnChangedMethod(nameof(OnMaxRChanged))]
		public double MaxR { get; set; }
		[OnChangedMethod(nameof(OnMinGChanged))]
		public double MinG { get; set; }
		[OnChangedMethod(nameof(OnMaxGChanged))]
		public double MaxG { get; set; }
		[OnChangedMethod(nameof(OnMinBChanged))]
		public double MinB { get; set; }
		[OnChangedMethod(nameof(OnMaxBChanged))]
		public double MaxB { get; set; }
		public ImageSource ImageSource => Image?.ToBitmapSource();

		public Image<Rgba, byte> Image { get; private set; }
		public Image<Rgba, byte> CroppedImage => model?.CroppedImage;

		public FilterPage()
		{
			InitializeComponent();
			Loaded += FilterPage_Loaded;
			Unloaded += FilterPage_Unloaded;
		}

		public FilterPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.CroppedImage)) {
				OnPropertyChanged(nameof(Enabled));
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(setting.FilterMin)) {
				MinR = setting.FilterMin.Red;
				MinG = setting.FilterMin.Green;
				MinB = setting.FilterMin.Blue;
			} else if (e.PropertyName == nameof(setting.FilterMax)) {
				MaxR = setting.FilterMax.Red;
				MaxG = setting.FilterMax.Green;
				MaxB = setting.FilterMax.Blue;
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void FilterPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!Enabled) {
				return;
			}
			FilterImage();
		}

		private void FilterPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!Enabled) {
				return;
			}
			model.Setting.FilterMin = new Rgba(MinR, MinG, MinB, byte.MaxValue);
			model.Setting.FilterMax = new Rgba(MaxR, MaxG, MaxB, byte.MaxValue);
		}
		private void FilterImage()
		{
			var min = new Rgba(MinR, MinG, MinB, byte.MaxValue);
			var max = new Rgba(MaxR, MaxG, MaxB, byte.MaxValue);
			Image = Methods.FilterRGB(CroppedImage, min, max);
		}

		private void OnMinRChanged() => FilterImage();
		private void OnMinGChanged() => FilterImage();
		private void OnMinBChanged() => FilterImage();
		private void OnMaxRChanged() => FilterImage();
		private void OnMaxGChanged() => FilterImage();
		private void OnMaxBChanged() => FilterImage();

	}
}
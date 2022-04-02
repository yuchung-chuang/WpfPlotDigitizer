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
	[AddINotifyPropertyChangedInterface]
	public partial class FilterPage : Page
	{
		private readonly Model model;
		public bool IsDisabled => model.CroppedImage is null;
		
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
		public Image<Rgba, byte> CroppedImage { get; private set; }

		public FilterPage()
		{
			InitializeComponent();
			Loaded += FilterPage_Loaded;
			Unloaded += FilterPage_Unloaded;
		}

		public FilterPage(Model model) : this()
		{
			this.model = model;
		}

		private void FilterPage_Loaded(object sender, RoutedEventArgs e)
		{
			IsEnabled = !IsDisabled;
			if (IsDisabled) {
				return;
			}
			CroppedImage = model.CroppedImage;
			MinR = model.FilterMin.Red;
			MinG = model.FilterMin.Green;
			MinB = model.FilterMin.Blue;
			MaxR = model.FilterMax.Red;
			MaxG = model.FilterMax.Green;
			MaxB = model.FilterMax.Blue;
			FilterImage();
		}

		private void FilterPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (IsDisabled) {
				return;
			}
			model.FilterMin = new Rgba(MinR, MinG, MinB, byte.MaxValue);
			model.FilterMax = new Rgba(MaxR, MaxG, MaxB, byte.MaxValue);
			model.FilterImage();
		}
		private void FilterImage()
		{
			if (IsDisabled) {
				return;
			}
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
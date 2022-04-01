using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for FilterPage.xaml
	/// </summary>
	public partial class FilterPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;

		public double MinR { get; set; } 
		public double MaxR { get; set; } 
		public double MinG { get; set; } 
		public double MaxG { get; set; } 
		public double MinB { get; set; } 
		public double MaxB { get; set; } 
		public ImageSource ImageSource => Image?.ToBitmapSource();

		public Image<Rgba, byte> Image { get; private set; }
		public Image<Rgba, byte> CroppedImage { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

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
			CroppedImage = model.CroppedImage;
			MinR = model.FilterMin.Red;
			MinG = model.FilterMin.Green;
			MinB = model.FilterMin.Blue;
			MaxR = model.FilterMax.Red;
			MaxG = model.FilterMax.Green;
			MaxB = model.FilterMax.Blue;
			FilterImage();
			PropertyChanged += FilterPage_PropertyChanged;
		}

		private void FilterPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.FilterMin = new Rgba(MinR, MinG, MinB, byte.MaxValue);
			model.FilterMax = new Rgba(MaxR, MaxG, MaxB, byte.MaxValue);
			model.FilterImage();
		}

		private void FilterPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if ((nameof(MinR) + nameof(MaxR) +
				nameof(MinG) + nameof(MaxG) +
				nameof(MinB) + nameof(MaxB)).Contains(e.PropertyName)) {
				FilterImage();
			}
		}

		private void FilterImage()
		{
			if (CroppedImage is null) {
				return;
			}
			var min = new Rgba(MinR, MinG, MinB, byte.MaxValue);
			var max = new Rgba(MaxR, MaxG, MaxB, byte.MaxValue);
			Image = Methods.FilterRGB(CroppedImage, min, max);
		}
	}
}
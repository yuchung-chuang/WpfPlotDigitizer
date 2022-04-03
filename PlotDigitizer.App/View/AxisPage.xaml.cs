using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Rectangle = System.Drawing.Rectangle;

namespace PlotDigitizer.App
{
	[AddINotifyPropertyChangedInterface]
	public partial class AxisPage : Page
	{
		private readonly Model model;

		public bool IsDisabled => model.InputImage is null;
		public ImageSource ImageSource { get; private set; }

		public double AxisLeft { get; set; }
		public double AxisTop { get; set; }
		public double AxisWidth { get; set; }
		public double AxisHeight { get; set; }

		public ICommand GetAxisCommand { get; set; }

		public AxisPage()
		{
			InitializeComponent();
			DataContext = this;
			GetAxisCommand = new RelayCommand(GetAxis);
			Loaded += AxisPage_Loaded;
			Unloaded += AxisPage_Unloaded;
		}

		public AxisPage(Model model) : this()
		{
			this.model = model;
		}

		private void AxisPage_Loaded(object sender, RoutedEventArgs e)
		{
			IsEnabled = !IsDisabled;
			if (IsDisabled) {
				return;
			}
			ImageSource = model.InputImage.ToBitmapSource();
			if (model.AxisLocation == default) {
				GetAxis();
			} else {
				AxisLeft = model.AxisLocation.Left;
				AxisTop = model.AxisLocation.Top;
				AxisWidth = model.AxisLocation.Width;
				AxisHeight = model.AxisLocation.Height;
			}
		}

		private void AxisPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (IsDisabled) {
				return;
			}
			model.AxisLocation = new Rectangle(
				(int)Math.Round(AxisLeft),
				(int)Math.Round(AxisTop),
				(int)Math.Round(AxisWidth),
				(int)Math.Round(AxisHeight));
		}

		private void GetAxis()
		{
			if (IsDisabled) {
				return;
			}
			var image = model.InputImage;
			var axis = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
			AxisLeft = axis.Left;
			AxisTop = axis.Top;
			AxisWidth = axis.Width;
			AxisHeight = axis.Height;
		}
	}
}
using GalaSoft.MvvmLight.Command;
using PlotDigitizer.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Rectangle = System.Drawing.Rectangle;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for AxisPage.xaml
	/// </summary>
	public partial class AxisPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;

		public event PropertyChangedEventHandler PropertyChanged;

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
			ImageSource = model.InputImage?.ToBitmapSource();
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
			model.AxisLocation = new Rectangle(
				(int)Math.Round(AxisLeft), 
				(int)Math.Round(AxisTop), 
				(int)Math.Round(AxisWidth),
				(int)Math.Round(AxisHeight));
			model.CropImage();
		}

		private void GetAxis()
		{
			var image = model.InputImage;
			if (image is null)
			{
				return;
			}
			var axis = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
			AxisLeft = axis.Left;
			AxisTop = axis.Top;
			AxisWidth = axis.Width;
			AxisHeight = axis.Height;
		}
	}
}
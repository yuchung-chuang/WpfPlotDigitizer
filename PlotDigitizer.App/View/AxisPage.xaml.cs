using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Rectangle = System.Drawing.Rectangle;

namespace PlotDigitizer.App
{
	public partial class AxisPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Enabled => model != null && model.InputImage != null;
		public ImageSource ImageSource => model?.InputImage?.ToBitmapSource();

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
			model.Setting.PropertyChanged += Setting_PropertyChanged;
			model.PropertyChanged += Model_PropertyChanged;
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(setting.AxisLocation)) {
				AxisLeft = model.Setting.AxisLocation.Left;
				AxisTop = model.Setting.AxisLocation.Top;
				AxisWidth = model.Setting.AxisLocation.Width;
				AxisHeight = model.Setting.AxisLocation.Height;
			}
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.InputImage)) {
				OnPropertyChanged(nameof(Enabled));
				OnPropertyChanged(nameof(ImageSource));
			}
		}

		private void AxisPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!Enabled) {
				return;
			}
			if (model.Setting.AxisLocation == default) {
				GetAxis();
			}
		}

		private void AxisPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!Enabled) {
				return;
			}
			model.Setting.AxisLocation = new Rectangle(
				(int)Math.Round(AxisLeft),
				(int)Math.Round(AxisTop),
				(int)Math.Round(AxisWidth),
				(int)Math.Round(AxisHeight));
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void GetAxis()
		{
			if (!Enabled) {
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
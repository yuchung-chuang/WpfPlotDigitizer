using PlotDigitizer.Core;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for AxisLimitPage.xaml
	/// </summary>
	public partial class AxisLimitPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;

		public AxisLimitPage()
		{
			InitializeComponent();
			Loaded += AxisLimitPage_Loaded;
			Unloaded += AxisLimitPage_Unloaded;
		}


		public AxisLimitPage(Model model) : this()
		{
			this.model = model;
		}
		private void AxisLimitPage_Loaded(object sender, RoutedEventArgs e)
		{
			ImageSource = model.InputImage?.ToBitmapSource();
			if (model.AxisLimit != default) {
				xMin = model.AxisLimit.Left;
				yMin = model.AxisLimit.Top;
				xMax = model.AxisLimit.Right;
				yMax = model.AxisLimit.Bottom;
			}
			if (model.AxisLogBase != default) {
				xLog = model.AxisLogBase.X;
				yLog = model.AxisLogBase.Y;
			}
		}

		private void AxisLimitPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.AxisLimit = new RectangleD(xMin ?? 0, yMin ?? 0, xMax - xMin ?? 0, yMax - yMin?? 0);
			model.AxisLogBase = new PointD(xLog ?? 0, yLog ?? 0);
		}

		public ImageSource ImageSource { get; private set; }

		private double? yMax = null;
		private double? yLog = null;
		private double? yMin = null;
		private double? xMax = null;
		private double? xLog = null;
		private double? xMin = null;

		public string AxisYMax
		{
			get => yMax.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					yMax = null;
				}
				else if (double.TryParse(value, out var result)) {
					yMax = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public string AxisYLog
		{
			get => yLog.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					yLog = null;
				}
				else if (double.TryParse(value, out var result)) {
					yLog = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public string AxisYMin
		{
			get => yMin.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					yMin = null;
				}
				else if (double.TryParse(value, out var result)) {
					yMin = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public string AxisXMax
		{
			get => xMax.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					xMax = null;
				}
				else if (double.TryParse(value, out var result)) {
					xMax = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public string AxisXLog
		{
			get => xLog.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					xLog = null;
				}
				else if (double.TryParse(value, out var result)) {
					xLog = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public string AxisXMin
		{
			get => xMin.ToString();
			set
			{
				if (string.IsNullOrWhiteSpace(value)) {
					xMin = null;
				}
				else if (double.TryParse(value, out var result)) {
					xMin = result;
				}
				else {
					MessageBox.Show("Cannot parse axis limit, please go back and check the values!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

	}
}
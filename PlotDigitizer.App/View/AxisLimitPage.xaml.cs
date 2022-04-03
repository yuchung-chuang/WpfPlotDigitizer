using PlotDigitizer.Core;
using PropertyChanged;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public partial class AxisLimitPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;
		public bool Enabled => model != null && model.InputImage != null;
		public ImageSource ImageSource => model?.InputImage?.ToBitmapSource();
		public AxisLimitPage()
		{
			InitializeComponent();
			Unloaded += AxisLimitPage_Unloaded;
		}

		public AxisLimitPage(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.InputImage)) {
				OnPropertyChanged(nameof(Enabled));
				OnPropertyChanged(nameof(ImageSource));
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(setting.AxisLimit)) {
				AxisXMin = setting.AxisLimit.Left.ToString();
				AxisYMin = setting.AxisLimit.Top.ToString();
				AxisXMax = setting.AxisLimit.Right.ToString();
				AxisYMax = setting.AxisLimit.Bottom.ToString();
			} else if (e.PropertyName == nameof(setting.AxisLogBase)) {
				AxisXLog = setting.AxisLogBase.X.ToString();
				AxisYLog = setting.AxisLogBase.Y.ToString();
			}
		}

		private void AxisLimitPage_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!Enabled) {
				return;
			}
			model.Setting.AxisLimit = new RectangleD(xMin ?? 0, yMin ?? 0, xMax - xMin ?? 0, yMax - yMin?? 0);
			model.Setting.AxisLogBase = new PointD(xLog ?? 0, yLog ?? 0);
		}

		

		private double? yMax = null;
		private double? yLog = null;
		private double? yMin = null;
		private double? xMax = null;
		private double? xLog = null;
		private double? xMin = null;

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

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


	}
}
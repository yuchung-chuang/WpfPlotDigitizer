using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class AxisLimitPageViewModel : PageViewModelBase
	{
		public double AxLimXMax { get; set; } = double.NaN;
		public double AxLimXMin { get; set; } = double.NaN;
		public double AxLimYMax { get; set; } = double.NaN;
		public double AxLimYMin { get; set; } = double.NaN;
		public double AxLimXLog { get; set; } = double.NaN;
		public double AxLimYLog { get; set; } = double.NaN;

		public RectangleD AxisLimit
		{
			get => new RectangleD(AxLimXMin, AxLimYMin, AxLimXMax - AxLimXMin, AxLimYMax - AxLimYMin);
			set
			{
				AxLimXMax = value.Right;
				AxLimXMin = value.Left;
				AxLimYMax = value.Bottom;
				AxLimYMin = value.Top;
			}
		}
		public PointD AxisLogBase
		{
			get => new PointD(AxLimXLog, AxLimYLog);
			set
			{
				AxLimXLog = value.X;
				AxLimYLog = value.Y;
			}
		}

		public bool IsEnabled => Model != null && Model.InputImage != null;

		public Model Model { get; }

		public AxisLimitPageViewModel()
		{
			Name = "AxisLimitPage";
		}
		public AxisLimitPageViewModel(Model model) : this()
		{
			Model = model;
			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(setting.AxisLimit)) {
				AxisLimit = setting.AxisLimit;
			}
			if (e.PropertyName == nameof(setting.AxisLogBase)) {
				AxisLogBase = setting.AxisLogBase;
			}
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Model.InputImage)) {
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public override void Enter()
		{
			base.Enter();

		}

		public override void Leave()
		{
			base.Leave();
			if (!IsEnabled) {
				return;
			}
			Model.Setting.AxisLimit = AxisLimit;
			Model.Setting.AxisLogBase = AxisLogBase;
		}
	}
}
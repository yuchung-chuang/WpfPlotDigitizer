using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class AxisLimitPageViewModel : PageViewModelBase
	{
		private readonly Setting setting;

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
		public AxisLimitPageViewModel(Model model, Setting setting) : this()
		{
			Model = model;
			this.setting = setting;
			model.PropertyChanged += Model_PropertyChanged;
			setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(Setting.AxisLimit)) {
				AxisLimit = setting.AxisLimit;
			}
			if (e.PropertyName == nameof(Setting.AxisLogBase)) {
				AxisLogBase = setting.AxisLogBase;
			}
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.InputImage)) {
				base.OnPropertyChanged(nameof(IsEnabled));
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
			setting.AxisLimit = AxisLimit;
			setting.AxisLogBase = AxisLogBase;
		}
	}
}
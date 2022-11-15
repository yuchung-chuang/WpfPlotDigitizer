using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class AxisLimitPageViewModel : PageViewModelBase
	{
		#region Fields

		private readonly Setting setting;

		#endregion Fields

		#region Properties

		public RectangleD AxisLimit
		{
			get => new(AxLimXMin, AxLimYMin, AxLimXMax - AxLimXMin, AxLimYMax - AxLimYMin);
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
			get => new(AxLimXLog, AxLimYLog);
			set
			{
				AxLimXLog = value.X;
				AxLimYLog = value.Y;
			}
		}

		public double AxLimXLog { get; set; } = double.NaN;
		public double AxLimXMax { get; set; } = double.NaN;
		public double AxLimXMin { get; set; } = double.NaN;
		public double AxLimYLog { get; set; } = double.NaN;
		public double AxLimYMax { get; set; } = double.NaN;
		public double AxLimYMin { get; set; } = double.NaN;
		public bool IsEnabled => Model != null && Model.InputImage != null;

		public Model Model { get; }

		#endregion Properties

		#region Constructors

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

		#endregion Constructors

		#region Methods

		public override void Enter()
		{
			base.Enter();
			if (!IsEnabled) {
				return;
			}
			AxisLimit = setting.AxisLimit;
			AxisLogBase = setting.AxisLogBase;
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

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.InputImage)) {
				base.OnPropertyChanged(nameof(IsEnabled));
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is not Setting setting) {
				return;
			}
			if (e.PropertyName == nameof(Setting.AxisLimit)) {
				AxisLimit = setting.AxisLimit;
			}
			if (e.PropertyName == nameof(Setting.AxisLogBase)) {
				AxisLogBase = setting.AxisLogBase;
			}
		}

		#endregion Methods
	}
}
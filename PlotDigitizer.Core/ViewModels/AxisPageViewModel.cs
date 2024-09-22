using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class AxisPageViewModel : PageViewModelBase
	{
		#region Fields

		private readonly Setting setting;
		private readonly IImageService imageService;

		#endregion Fields

		#region Properties

		public double AxisHeight { get; set; }
		public double AxisLeft { get; set; }
		public double AxisTop { get; set; }
		public double AxisWidth { get; set; }
		public RectangleD AxisLocation
		{
			get => new(AxisLeft, AxisTop, AxisWidth, AxisHeight);
			set
			{
				AxisLeft = value.Left;
				AxisTop = value.Top;
				AxisWidth = value.Width;
				AxisHeight = value.Height;
			}
		}
		public RelayCommand GetAxisCommand { get; set; }
		public bool IsEnabled => Model != null && Model.InputImage != null;
		public Model Model { get; }
        public Image<Rgba, byte> Image => !IsEnabled ? null : Model.InputImage;

        #endregion Properties

        #region Constructors

        public AxisPageViewModel()
		{
			Name = "AxisPage";
			GetAxisCommand = new RelayCommand(GetAxis, CanGetAxis);
		}

		public AxisPageViewModel(Model model, 
			Setting setting, 
			IImageService imageService) : this()
		{
			Model = model;
			this.setting = setting;
			this.imageService = imageService;
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
			if (setting.AxisLocation == default) {
				if (GetAxisCommand.CanExecute()) {
					GetAxisCommand.Execute();
				}
			} else {
				AxisLocation = setting.AxisLocation;
			}
		}

		public override void Leave()
		{
			base.Leave();
			if (!IsEnabled) {
				return;
			}
			setting.AxisLocation = AxisLocation;
		}

		private bool CanGetAxis() => IsEnabled;

		private void GetAxis()
		{
			var image = Model.InputImage;
			AxisLocation = imageService.GetAxisLocation(image) ??
				new RectangleD(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Core.Model.InputImage)) {
				base.OnPropertyChanged(nameof(IsEnabled));
				GetAxisCommand.RaiseCanExecuteChanged();
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Setting.AxisLocation)) {
				AxisLocation = setting.AxisLocation;
			}
		}

		#endregion Methods
	}
}
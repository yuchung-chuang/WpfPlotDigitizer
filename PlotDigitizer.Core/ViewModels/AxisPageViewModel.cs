using System;
using System.ComponentModel;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public class AxisPageViewModel : PageViewModelBase
	{
		public double AxisLeft { get; set; }
		public double AxisTop { get; set; }
		public double AxisWidth { get; set; }
		public double AxisHeight { get; set; }
		public Rectangle AxisLocation
		{
			get => new Rectangle(
				(int)Math.Round(AxisLeft),
				(int)Math.Round(AxisTop),
				(int)Math.Round(AxisWidth),
				(int)Math.Round(AxisHeight));
			set
			{
				AxisLeft = value.Left;
				AxisTop = value.Top;
				AxisWidth = value.Width;
				AxisHeight = value.Height;
			}
		}
		public Model Model { get; }
		public bool IsEnabled => Model != null && Model.InputImage != null;

		public RelayCommand GetAxisCommand { get; set; }
		public AxisPageViewModel()
		{
			Name = "AxisPage";
			GetAxisCommand = new RelayCommand(GetAxis, CanGetAxis);
		}

		public AxisPageViewModel(Model model) : this()
		{
			Model = model;
			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Setting.AxisLocation)) {
				AxisLocation = Model.Setting.AxisLocation;
			}
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Model.InputImage)) {
				OnPropertyChanged(nameof(IsEnabled));
				GetAxisCommand.RaiseCanExecuteChanged();
			}
		}

		private bool CanGetAxis() => IsEnabled;

		private void GetAxis()
		{
			var image = Model.InputImage;
			AxisLocation = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
		}

		public override void Enter()
		{
			base.Enter();
			if (!IsEnabled) {
				return;
			}
			if (Model.Setting.AxisLocation == default) {
				if (GetAxisCommand.CanExecute()) {
					GetAxisCommand.Execute();
				}
			}
		}

		public override void Leave()
		{
			base.Leave();
			if (!IsEnabled) {
				return;
			}
			Model.Setting.AxisLocation = AxisLocation;
		}
	}
}

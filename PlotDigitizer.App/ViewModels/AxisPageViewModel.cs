using PlotDigitizer.Core;
using System.ComponentModel;
using System.Drawing;

namespace PlotDigitizer.App
{
	public class AxisPageViewModel : ViewModelBase
	{
		public Model Model { get; }
		public bool IsEnabled => Model != null && Model.InputImage != null;

		public RelayCommand GetAxisCommand { get; set; }
		public AxisPageViewModel()
		{
			GetAxisCommand = new RelayCommand(GetAxis, CanGetAxis);
		}

		public AxisPageViewModel(Model model) : this()
		{
			Model = model;
			model.PropertyChanged += Model_PropertyChanged;

		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Model.InputImage)) {
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		private bool CanGetAxis() => IsEnabled;

		private void GetAxis()
		{
			var image = Model.InputImage;
			var axis = Methods.GetAxisLocation(image) ?? new Rectangle(image.Width / 4, image.Height / 4, image.Width / 2, image.Height / 2);
			Model.Setting.AxisLocation = axis;
		}
	}
}

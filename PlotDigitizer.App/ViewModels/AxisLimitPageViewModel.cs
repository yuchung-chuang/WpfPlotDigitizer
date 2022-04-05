using PlotDigitizer.Core;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public class AxisLimitPageViewModel : ViewModelBase
	{
		public bool Enabled => Model != null && Model.InputImage != null;

		public Model Model { get; }

		public AxisLimitPageViewModel()
		{

		}
		public AxisLimitPageViewModel(Model model) : this()
		{
			Model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}
		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Model.InputImage)) {
				OnPropertyChanged(nameof(Enabled));
			}
		}
	}
}
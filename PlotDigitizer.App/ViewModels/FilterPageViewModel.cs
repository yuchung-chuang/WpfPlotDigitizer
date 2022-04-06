using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PlotDigitizer.App
{
	public class FilterPageViewModel : ViewModelBase
	{
		public bool IsEnabled => Model != null && Model.CroppedImage != null;

		public Image<Rgba, byte> Image { get; private set; }
		public Image<Rgba, byte> CroppedImage => Model?.CroppedImage;
		public Model Model { get; }
		public FilterPageViewModel()
		{

		}

		public FilterPageViewModel(Model model) : this()
		{
			Model = model;

			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Model.CroppedImage)) {
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			//if (e.PropertyName == nameof(setting.FilterMin)) {
			//	MinR = setting.FilterMin.Red;
			//	MinG = setting.FilterMin.Green;
			//	MinB = setting.FilterMin.Blue;
			//} else if (e.PropertyName == nameof(setting.FilterMax)) {
			//	MaxR = setting.FilterMax.Red;
			//	MaxG = setting.FilterMax.Green;
			//	MaxB = setting.FilterMax.Blue;
			//}
		}

		public void FilterImage()
		{
			if (!IsEnabled) {
				return;
			}
			//Image = Methods.FilterRGB(CroppedImage, FilterMin, FilterMax);
		}

		private void OnMinRChanged() => FilterImage();
		private void OnMinGChanged() => FilterImage();
		private void OnMinBChanged() => FilterImage();
		private void OnMaxRChanged() => FilterImage();
		private void OnMaxGChanged() => FilterImage();
		private void OnMaxBChanged() => FilterImage();
	}
}

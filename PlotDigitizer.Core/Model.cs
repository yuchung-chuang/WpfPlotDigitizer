using Emgu.CV;
using Emgu.CV.Structure;
using PropertyChanged;
using System.ComponentModel;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public class Model : INotifyPropertyChanged
	{
		[OnChangedMethod(nameof(OnInputImageChanged))]
		public Image<Rgba, byte> InputImage { get; set; }

		[OnChangedMethod(nameof(OnCroppedImageChanged))]
		public Image<Rgba, byte> CroppedImage { get; set; }
		
		[OnChangedMethod(nameof(OnFilteredImageChanged))]
		public Image<Rgba, byte> FilteredImage { get; set; }
		
		public Image<Rgba, byte> EdittedImage { get; set; }

		public Setting Setting { get; private set; } = new Setting();

		public event PropertyChangedEventHandler PropertyChanged;

		public Model()
		{
			Setting.PropertyChanged += Setting_PropertyChanged;	
		}

		public void Load(Setting setting)
		{
			foreach (var property in typeof(Setting).GetProperties()) {
				var value = property.GetValue(setting);
				if (value != default) {
					property.SetValue(Setting, value);
				}
			}
		}

		private void OnInputImageChanged()
		{
			if (Setting.AxisLocation == default) {
				Setting.AxisLocation = Methods.GetAxisLocation(InputImage) ?? new Rectangle(InputImage.Width / 4, InputImage.Height / 4, InputImage.Width / 2, InputImage.Height / 2); 
			}
			CropImage();
		}
		private void OnCroppedImageChanged()
		{
			FilterImage();
		}
		private void OnFilteredImageChanged() => EdittedImage = FilteredImage;

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InputImage is null) {
				return;
			}
			switch (e.PropertyName) {
				case nameof(Setting.AxisLocation):
					CropImage();
					break;
				case nameof(Setting.FilterMin):
					FilterImage();
					break;
				case nameof(Setting.FilterMax):
					FilterImage();
					break;
				default:
					break;
			}
		}


		private void CropImage()
		{
			CroppedImage = Methods.CropImage(InputImage, Setting.AxisLocation);
		}

		private void FilterImage()
		{
			FilteredImage = Methods.FilterRGB(CroppedImage, Setting.FilterMin, Setting.FilterMax);
		}
	}
}
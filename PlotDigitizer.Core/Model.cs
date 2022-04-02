using Emgu.CV;
using Emgu.CV.Structure;
using PropertyChanged;
using System.ComponentModel;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public class Model : INotifyPropertyChanged
	{
		public Image<Rgba, byte> InputImage { get; set; }
		public RectangleD AxisLimit { get; set; }
		public PointD AxisLogBase { get; set; }
		[OnChangedMethod(nameof(OnAxisLocationChanged))]
		public Rectangle AxisLocation { get; set; }
		public Image<Rgba, byte> CroppedImage { get; set; }
		[OnChangedMethod(nameof(OnFilterMinChanged))]
		public Rgba FilterMin { get; set; } = new Rgba(0, 0, 0, byte.MaxValue);
		[OnChangedMethod(nameof(OnFilterMaxChanged))]
		public Rgba FilterMax { get; set; } = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
		[OnChangedMethod(nameof(OnFilteredImageChanged))]
		public Image<Rgba, byte> FilteredImage { get; set; }
		public Image<Rgba, byte> EdittedImage { get; set; }
		public DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnAxisLocationChanged() => CropImage();

		private void OnFilterMinChanged() => FilterImage();

		private void OnFilterMaxChanged() => FilterImage();

		private void OnFilteredImageChanged() => EdittedImage = FilteredImage;

		private void CropImage()
		{
			CroppedImage = Methods.CropImage(InputImage, AxisLocation);
		}

		private void FilterImage()
		{
			FilteredImage = Methods.FilterRGB(CroppedImage, FilterMin, FilterMax);
		}
	}

	public enum DataType
	{
		Continuous,
		Discrete,
	}
}
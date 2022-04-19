using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class Model : INotifyPropertyChanged
	{
		public virtual Image<Rgba, byte> InputImage { get; set; }
		public virtual Image<Rgba, byte> CroppedImage { get; protected set; }
		public virtual Image<Rgba, byte> FilteredImage { get; protected set; }
		public virtual Image<Rgba, byte> EdittedImage { get; set; }
		public virtual Image<Rgba, byte> PreviewImage { get; protected set; }
		public virtual IEnumerable<PointD> Data { get; protected set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
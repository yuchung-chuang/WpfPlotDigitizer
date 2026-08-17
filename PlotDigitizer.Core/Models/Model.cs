using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public class Model : INotifyPropertyChanged
	{
		public virtual Image<Rgba, byte> InputImage { get; set; }
		public virtual Image<Rgba, byte> CroppedImage { get; set; }
		public virtual Image<Rgba, byte> FilteredImage { get; set; }
		public virtual Image<Rgba, byte> EditedImage { get; set; }
		public virtual IEnumerable<PointD> DataPoints { get; set; }
		public virtual IEnumerable<PointD> Data { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<string> PropertyOutdated;

		protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		protected virtual void OnPropertyOutdated(string propertyName) => PropertyOutdated?.Invoke(this, propertyName);
	}
}
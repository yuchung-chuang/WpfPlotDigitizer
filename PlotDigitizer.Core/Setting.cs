using Emgu.CV.Structure;
using System;
using System.ComponentModel;
using System.Drawing;

namespace PlotDigitizer.Core
{
	[Serializable]
	public class Setting : INotifyPropertyChanged
	{
		public RectangleD AxisLimit { get; set; }
		public PointD AxisLogBase { get; set; }

		public Rectangle AxisLocation { get; set; }

		public Rgba FilterMin { get; set; } = new Rgba(0, 0, 0, byte.MaxValue);

		public Rgba FilterMax { get; set; } = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);

		public DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
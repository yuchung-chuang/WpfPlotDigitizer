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

		public Rgba FilterMin { get; set; }

		public Rgba FilterMax { get; set; }

		public DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
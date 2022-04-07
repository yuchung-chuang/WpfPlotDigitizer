using Emgu.CV.Structure;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

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
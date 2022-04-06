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
		public double AxLimXMax { get; set; } = double.NaN;
		public double AxLimXMin { get; set; } = double.NaN;
		public double AxLimYMax { get; set; } = double.NaN;
		public double AxLimYMin { get; set; } = double.NaN;
		public double AxLimXLog { get; set; } = double.NaN;
		public double AxLimYLog { get; set; } = double.NaN;

		[JsonIgnore]
		[XmlIgnore]
		public double AxLimXLength => AxLimXMax - AxLimXMin;

		[JsonIgnore]
		[XmlIgnore]
		public double AxLimYLength => AxLimYMax - AxLimYMin;

		[JsonIgnore]
		[XmlIgnore]
		public RectangleD AxisLimit
		{
			get => new RectangleD(AxLimXMin, AxLimYMin, AxLimXLength, AxLimYLength);
			set
			{
				AxLimXMin = value.X;
				AxLimYMin = value.Y;
				AxLimXMax = value.Right;
				AxLimYMax = value.Bottom;
			}
		}

		[JsonIgnore]
		[XmlIgnore]
		public PointD AxisLogBase
		{
			get => new PointD(AxLimXLog, AxLimYLog);
			set
			{
				AxLimXLog = value.X;
				AxLimYLog = value.Y;
			}
		}

		public double AxisLeft { get; set; }
		public double AxisTop { get; set; }
		public double AxisWidth { get; set; }
		public double AxisHeight { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public Rectangle AxisLocation
		{
			get => new Rectangle((int)Math.Round(AxisLeft),
								(int)Math.Round(AxisTop),
								(int)Math.Round(AxisWidth),
								(int)Math.Round(AxisHeight));
			set
			{
				AxisLeft = value.Left;
				AxisTop = value.Top;
				AxisWidth = value.Width;
				AxisHeight = value.Height;
				OnAxisLocationSetted();
			}
		}

		public event EventHandler AxisLocationSetted;

		private void OnAxisLocationSetted()
		{
			AxisLocationSetted?.Invoke(this, EventArgs.Empty);
		}

		public Rgba FilterMin { get; set; } = new Rgba(0, 0, 0, byte.MaxValue);

		public Rgba FilterMax { get; set; } = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);

		public DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
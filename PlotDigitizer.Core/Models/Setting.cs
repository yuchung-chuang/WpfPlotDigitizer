using Emgu.CV.Structure;
using System;
using System.ComponentModel;
using System.Drawing;

namespace PlotDigitizer.Core
{
	[Serializable]
	public class Setting : INotifyPropertyChanged
	{
		public virtual RectangleD AxisLimit { get; set; }
		public virtual Rectangle AxisLocation { get; set; }
		public virtual PointD AxisLogBase { get; set; }
		public virtual Rgba FilterMax { get; set; }
		public virtual Rgba FilterMin { get; set; }
		public virtual DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public Setting Copy()
		{
			return new Setting
			{
				AxisLimit = AxisLimit,
				AxisLogBase = AxisLogBase,
				AxisLocation = AxisLocation,
				FilterMin = FilterMin,
				FilterMax = FilterMax,
				DataType = DataType,
			};
		}

		public void Load(Setting setting)
		{
			foreach (var property in typeof(Setting).GetProperties()) {
				var value = property.GetValue(setting);
				if (value != default) {
					property.SetValue(this, value);
				}
			}
		}
	}
}
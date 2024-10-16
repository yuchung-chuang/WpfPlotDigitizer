﻿using Emgu.CV.Structure;

using System;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
    [Serializable]
	public class Setting : INotifyPropertyChanged
	{
		public virtual RectangleD AxisLocation { get; set; }
		public virtual AxisLimitTextBoxD AxisTextBox { get; set; }
		public virtual RectangleD AxisLimit { get; set; }
		public virtual AxisTitle AxisTitle { get; set; }
		public virtual PointD AxisLogBase { get; set; }
		public virtual Rgba FilterMax { get; set; }
		public virtual Rgba FilterMin { get; set; }
		public virtual DataType DataType { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<string> PropertyOutdated;
		public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public void RaisePropertyOutdated(string propertyName) => PropertyOutdated?.Invoke(this, propertyName);

		public Setting Copy()
		{
			return new Setting
			{
				AxisLocation = AxisLocation,
				AxisTextBox = AxisTextBox,
				AxisLimit = AxisLimit,
				AxisTitle = AxisTitle,
				AxisLogBase = AxisLogBase,
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
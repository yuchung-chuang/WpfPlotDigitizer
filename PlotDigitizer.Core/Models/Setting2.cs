using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace PlotDigitizer.Core
{
	[Serializable]
	public class Setting2 : ISetting
	{
		private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;
		private readonly AxisLocationNode axisLocation;
		private readonly FilterMinNode filterMin;
		private readonly FilterMaxNode filterMax;
		private readonly DataTypeNode dataType;

		public Setting2(AxisLimitNode axisLimit, AxisLogBaseNode axisLogBase, AxisLocationNode axisLocation, FilterMinNode filterMin, FilterMaxNode filterMax, DataTypeNode dataType)
		{
			this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			this.axisLocation = axisLocation;
			this.filterMin = filterMin;
			this.filterMax = filterMax;
			this.dataType = dataType;
		}

		public void Load(ISetting setting)
		{
			foreach (var property in typeof(ISetting).GetProperties()) {
				var value = property.GetValue(setting);
				if (value != default) {
					property.SetValue(this, value);
				}
			}
		}

		public RectangleD AxisLimit
		{
			get => axisLimit.Get();
			set => axisLimit.Set(value);
		}
		public PointD AxisLogBase
		{
			get => axisLogBase.Get();
			set => axisLogBase.Set(value);
		}
		public Rectangle AxisLocation
		{
			get => axisLocation.Get();
			set => axisLocation.Set(value);
		}
		public Rgba FilterMin
		{
			get => filterMin.Get();
			set => filterMin.Set(value);
		}
		public Rgba FilterMax
		{
			get => filterMax.Get();
			set => filterMax.Set(value);
		}
		public DataType DataType
		{
			get => dataType.Get();
			set => dataType.Set(value);
		}
	}
}
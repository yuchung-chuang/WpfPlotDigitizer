using Emgu.CV.Structure;

using System.Drawing;

namespace PlotDigitizer.Core
{
	public class SettingFacade : Setting
	{
		private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;
		private readonly AxisLocationNode axisLocation;
		private readonly FilterMinNode filterMin;
		private readonly FilterMaxNode filterMax;
		private readonly DataTypeNode dataType;

		public SettingFacade(AxisLimitNode axisLimit, AxisLogBaseNode axisLogBase, AxisLocationNode axisLocation, FilterMinNode filterMin, FilterMaxNode filterMax, DataTypeNode dataType)
		{
			this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			this.axisLocation = axisLocation;
			this.filterMin = filterMin;
			this.filterMax = filterMax;
			this.dataType = dataType;

			axisLimit.Updated += (s, e) => RaisePropetyChanged(nameof(AxisLimit));
			axisLogBase.Updated += (s, e) => RaisePropetyChanged(nameof(AxisLogBase));
			axisLocation.Updated += (s, e) => RaisePropetyChanged(nameof(AxisLocation));
			filterMin.Updated += (s, e) => RaisePropetyChanged(nameof(FilterMin));
			filterMax.Updated += (s, e) => RaisePropetyChanged(nameof(FilterMax));
			dataType.Updated += (s, e) => RaisePropetyChanged(nameof(DataType));
		}

		public override RectangleD AxisLimit
		{
			get => axisLimit.Get();
			set => axisLimit.Set(value);
		}

		public override PointD AxisLogBase
		{
			get => axisLogBase.Get();
			set => axisLogBase.Set(value);
		}

		public override Rectangle AxisLocation
		{
			get => axisLocation.Get();
			set => axisLocation.Set(value);
		}

		public override Rgba FilterMin
		{
			get => filterMin.Get();
			set => filterMin.Set(value);
		}

		public override Rgba FilterMax
		{
			get => filterMax.Get();
			set => filterMax.Set(value);
		}

		public override DataType DataType
		{
			get => dataType.Get();
			set => dataType.Set(value);
		}
	}
}
using Emgu.CV.Structure;

using System.Drawing;

namespace PlotDigitizer.Core
{
	public class UpdatableSetting : Setting
	{
		private readonly AxisLocationNode axisLocation;
        private readonly AxisLimitTextBoxNode axisLimitTextBox;
        private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;
		private readonly FilterMinNode filterMin;
		private readonly FilterMaxNode filterMax;
		private readonly DataTypeNode dataType;

		public UpdatableSetting(
			AxisLocationNode axisLocation, 
			AxisLimitTextBoxNode axisLimitTextBox,
			AxisLimitNode axisLimit, 
			AxisLogBaseNode axisLogBase, 
			FilterMinNode filterMin, 
			FilterMaxNode filterMax, 
			DataTypeNode dataType)
		{
			this.axisLocation = axisLocation;
            this.axisLimitTextBox = axisLimitTextBox;
            this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			this.filterMin = filterMin;
			this.filterMax = filterMax;
			this.dataType = dataType;

			axisLocation.Updated += (s, e) => RaisePropertyChanged(nameof(AxisLocation));
			axisLimitTextBox.Updated += (s, e) => RaisePropertyChanged(nameof(AxisLimitTextBox));
            axisLimit.Updated += (s, e) => RaisePropertyChanged(nameof(AxisLimit));
			axisLogBase.Updated += (s, e) => RaisePropertyChanged(nameof(AxisLogBase));
			filterMin.Updated += (s, e) => RaisePropertyChanged(nameof(FilterMin));
			filterMax.Updated += (s, e) => RaisePropertyChanged(nameof(FilterMax));
			dataType.Updated += (s, e) => RaisePropertyChanged(nameof(DataType));

			axisLocation.Outdated += (s, e) => RaisePropertyOutdated(nameof(AxisLocation));
			axisLimitTextBox.Outdated += (s, e) => RaisePropertyChanged(nameof(AxisLimitTextBox));
            axisLimit.Outdated+= (s, e) => RaisePropertyOutdated(nameof(AxisLimit));
			axisLogBase.Outdated+= (s, e) => RaisePropertyOutdated(nameof(AxisLogBase));
			filterMin.Outdated+= (s, e) => RaisePropertyOutdated(nameof(FilterMin));
			filterMax.Outdated+= (s, e) => RaisePropertyOutdated(nameof(FilterMax));
			dataType.Outdated+= (s, e) => RaisePropertyOutdated(nameof(DataType));
		}
		public override RectangleD AxisLocation
		{
			get => axisLocation.Get();
			set => axisLocation.Set(value);
		}

        public override AxisLimitTextBoxD AxisLimitTextBox 
		{ 
			get => axisLimitTextBox.Get();
			set => axisLimitTextBox.Set(value); 
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
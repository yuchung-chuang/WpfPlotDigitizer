using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
    public class UpdatableSetting : Setting
    {
        private readonly AxisLocationNode axisLocation;
        private readonly AxisTextBoxNode axisTextBox;
        private readonly AxisLimitNode axisLimit;
        private readonly AxisTitleNode axisTitle;
        private readonly AxisLogBaseNode axisLogBase;
        private readonly FilterMinNode filterMin;
        private readonly FilterMaxNode filterMax;
        private readonly DataTypeNode dataType;

        public UpdatableSetting(
            AxisLocationNode axisLocation,
            AxisTextBoxNode axisTextBox,
            AxisLimitNode axisLimit,
            AxisTitleNode axisTitle,
            AxisLogBaseNode axisLogBase,
            FilterMinNode filterMin,
            FilterMaxNode filterMax,
            DataTypeNode dataType)
        {
            this.axisLocation = axisLocation;
            this.axisTextBox = axisTextBox;
            this.axisLimit = axisLimit;
            this.axisTitle = axisTitle;
            this.axisLogBase = axisLogBase;
            this.filterMin = filterMin;
            this.filterMax = filterMax;
            this.dataType = dataType;

            RelayEvents(axisLocation, nameof(AxisLocation));
            RelayEvents(axisTextBox, nameof(AxisTextBox));
            RelayEvents(axisLimit, nameof(AxisLimit));
            RelayEvents(axisTitle, nameof(AxisTitle));
            RelayEvents(axisLogBase, nameof(AxisLogBase));
            RelayEvents(filterMin, nameof(FilterMin));
            RelayEvents(filterMax, nameof(FilterMax));
            RelayEvents(dataType, nameof(DataType));
        }

        private void RelayEvents(UpdatableNode node, string propertyName)
        {
            node.Updated += (s, e) => OnPropertyChanged(propertyName);
            node.Outdated += (s, e) => OnPropertyOutdated(propertyName);
        }

        public override RectangleD AxisLocation
        {
            get => axisLocation.Get();
            set => axisLocation.Set(value);
        }
        public override AxisLimitTextBoxD AxisTextBox
        {
            get => axisTextBox.Get();
            set => axisTextBox.Set(value);
        }
        public override RectangleD AxisLimit
        {
            get => axisLimit.Get();
            set => axisLimit.Set(value);
        }
        public override AxisTitle AxisTitle
        {
            get => axisTitle.Get();
            set => axisTitle.Set(value);
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
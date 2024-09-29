namespace PlotDigitizer.Core
{
    public class AxisTextBoxNode : UpdatableNode<AxisLimitTextBoxD>
    {
        private readonly AxisLocationNode axisLocation;

        public AxisTextBoxNode(AxisLocationNode axisLocation)
        {
            this.axisLocation = axisLocation;
            axisLocation.Updated += (s, e) => OnOutdated();
            axisLocation.Outdated += (s, e) => OnOutdated();
        }

        public override void Update()
        {
            if (!axisLocation.CheckUpdate())
                return;
            Value = default;
            base.OnUpdated();
        }
    }
    
}

namespace PlotDigitizer.Core
{
    public class AxisTextBoxNode : UpdatableNode<AxisLimitTextBoxD>
    {
        public AxisTextBoxNode(AxisLocationNode axisLocation)
        {
            DependsOn(axisLocation);
        }

        protected override void Update()
        {
            if (!IsAllDependenciesUpdated())
                return;
            Data = default;
        }
    }
    
}

namespace PlotDigitizer.Core
{
    public abstract class DependOnAxisTextBoxNode<TData> : UpdatableNode<TData>
    {
        public DependOnAxisTextBoxNode(AxisTextBoxNode axisTextBox)
        {
            DependsOn(axisTextBox);
        }

        protected override void Update()
        {
            if (!IsAllDependenciesUpdated()) {
                return;
            }
            Data = default;
        }
    }
}
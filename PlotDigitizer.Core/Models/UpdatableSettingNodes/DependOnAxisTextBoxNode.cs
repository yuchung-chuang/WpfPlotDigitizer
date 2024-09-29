namespace PlotDigitizer.Core
{
    public abstract class DependOnAxisTextBoxNode<TData> : UpdatableNode<TData>
    {
        private readonly AxisTextBoxNode axisTextBox;

        public DependOnAxisTextBoxNode(AxisTextBoxNode axisTextBox)
        {
            this.axisTextBox = axisTextBox;
            axisTextBox.Updated += (s, e) => OnOutdated();
            axisTextBox.Outdated += (s, e) => OnOutdated();
        }

        public override void Update()
        {
            if (!axisTextBox.CheckUpdate()) {
                return;
            }
            Value = default;
            base.Update();
        }
    }
}
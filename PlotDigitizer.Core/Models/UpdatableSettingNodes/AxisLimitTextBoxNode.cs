using System;
using System.Collections.Generic;
using System.Text;

namespace PlotDigitizer.Core
{
    public class AxisLimitTextBoxNode : UpdatableNode<AxisLimitTextBoxD>
    {
        private readonly AxisLocationNode axisLocation;

        public AxisLimitTextBoxNode(AxisLocationNode axisLocation)
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

﻿namespace PlotDigitizer.Core
{
	public class AxisLimitNode : UpdatableNode<RectangleD>
	{
		private readonly InputImageNode inputImage;

		public AxisLimitNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
			inputImage.Outdated += (s, e) => OnOutdated();
        }

        public override void Update()
		{
			if (!inputImage.CheckUpdate()) {
				return;
			}
			Value = default;
			base.Update();
		}
	}
}
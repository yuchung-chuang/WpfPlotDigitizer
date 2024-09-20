namespace PlotDigitizer.Core
{
	public class AxisLimitNode : ModelNode<RectangleD>
	{
		private readonly InputImageNode inputImage;

		public AxisLimitNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			Value = default;
			base.Update();
		}
	}
}
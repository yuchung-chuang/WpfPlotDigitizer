namespace PlotDigitizer.Core
{
	public class AxisLimitNode : UpdatableNode<RectangleD>
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
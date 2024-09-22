namespace PlotDigitizer.Core
{
	public class AxisLogBaseNode : UpdatableNode<PointD>
	{

		private readonly InputImageNode inputImage;

		public AxisLogBaseNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
			IsUpdated = true;
		}

		public override void Update()
		{
			Value = default;
			base.Update();
		}
	}
}
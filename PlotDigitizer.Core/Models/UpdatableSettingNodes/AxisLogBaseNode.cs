namespace PlotDigitizer.Core
{
	public class AxisLogBaseNode : UpdatableNode<PointD>
	{
		private readonly InputImageNode inputImage;

		public AxisLogBaseNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
			inputImage.Outdated += (s, e) => OnOutdated();
            IsUpdated = true;
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate())
				return;
			Value = default;
			base.Update();
		}
	}
}
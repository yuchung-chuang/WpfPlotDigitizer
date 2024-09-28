namespace PlotDigitizer.Core
{
	public class AxisLimitNode : UpdatableNode<RectangleD>
	{
		private readonly InputImageNode inputImage;
		private readonly AxisLocationNode axisLocation;
		private readonly IImageService imageService;

		public AxisLimitNode(InputImageNode inputImage, AxisLocationNode axisLocation, IImageService imageService)
		{
			this.inputImage = inputImage;
			this.axisLocation = axisLocation;
			this.imageService = imageService;
			inputImage.Updated += (s, e) => OnOutdated();
			axisLocation.Updated += (s, e) => OnOutdated();
			inputImage.Outdated += (s, e) => OnOutdated();
        }

		public override void Update()
		{
			if (!inputImage.CheckUpdate() || !axisLocation.CheckUpdate()) {
				return;
			}
			Value = default;
			base.Update();
		}
	}
}
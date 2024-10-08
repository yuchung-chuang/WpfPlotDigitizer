using Emgu.CV;
using Emgu.CV.Structure;

#nullable enable

namespace PlotDigitizer.Core
{
	public class CroppedImageNode : UpdatableNode<Image<Rgba, byte>>
	{
		private readonly InputImageNode inputImage;
		private readonly AxisLocationNode axisLocation;
		private readonly IImageService imageService;

		public CroppedImageNode(InputImageNode inputImage, 
			AxisLocationNode axisLocation,
			IImageService imageService)
		{
			this.inputImage = inputImage;
			this.axisLocation = axisLocation;
			this.imageService = imageService;
			inputImage.Updated += (s, e) => OnOutdated();
			inputImage.Outdated += (s, e) => OnOutdated();
			axisLocation.Updated += (s, e) => OnOutdated();
			axisLocation.Outdated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate() || !axisLocation.CheckUpdate())
				return;
			if (inputImage.Value is null)
				return;
			Value = imageService.CropImage(inputImage.Value, axisLocation.Value);
			base.Update();
		}

	}
}
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
			DependsOn(inputImage);
			DependsOn(axisLocation);
		}

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated())
				return;
			if (inputImage.Data is null)
				return;
			Data = imageService.CropImage(inputImage.Data, axisLocation.Data);
		}

	}
}
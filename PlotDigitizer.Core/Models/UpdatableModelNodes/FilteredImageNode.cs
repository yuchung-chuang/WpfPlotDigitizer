using Emgu.CV;
using Emgu.CV.Structure;

#nullable enable
namespace PlotDigitizer.Core
{
	public class FilteredImageNode : UpdatableNode<Image<Rgba, byte>>
	{
		private readonly CroppedImageNode croppedImage;
		private readonly FilterMinNode filterMin;
		private readonly FilterMaxNode filterMax;
		private readonly IImageService imageService;

		public FilteredImageNode(CroppedImageNode croppedImage, 
			FilterMinNode filterMin, 
			FilterMaxNode filterMax, 
			IImageService imageService)
		{
			this.croppedImage = croppedImage;
			this.filterMin = filterMin;
			this.filterMax = filterMax;
			this.imageService = imageService;
			DependsOn(croppedImage);
			DependsOn(filterMin);
			DependsOn(filterMax);
		}

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated()) {
				return;
			}
			if (croppedImage.Data is null)
				return;
            Data = imageService.FilterRGB(croppedImage.Data, filterMin.Data, filterMax.Data);
		}
	}
}
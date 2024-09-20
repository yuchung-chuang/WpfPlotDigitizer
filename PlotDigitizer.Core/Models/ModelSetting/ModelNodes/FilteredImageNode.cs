using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilteredImageNode : ModelNode<Image<Rgba, byte>>
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
			croppedImage.Updated += (s, e) => OnOutdated();
			croppedImage.Outdated += (s, e) => OnOutdated();
			filterMin.Updated += (s, e) => OnOutdated();
			filterMin.Outdated += (s, e) => OnOutdated();
			filterMax.Updated += (s, e) => OnOutdated();
			filterMax.Outdated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!croppedImage.CheckUpdate() || !filterMin.CheckUpdate() || !filterMax.CheckUpdate()) {
				return;
			}
			Value = imageService.FilterRGB(croppedImage.Value, filterMin.Value, filterMax.Value);
			base.Update();
		}
	}
}
using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class EdittedImageNode : UpdatableNode<Image<Rgba, byte>>
	{
		private readonly FilteredImageNode filteredImage;

		public EdittedImageNode(FilteredImageNode filteredImage)
		{
			this.filteredImage = filteredImage;
			DependsOn(filteredImage);
		}

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated())
				return;
			Data = filteredImage.Data?.Copy();
			OnUpdated();
		}
	}
}
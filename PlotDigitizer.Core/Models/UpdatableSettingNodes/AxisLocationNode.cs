using System.Drawing;

namespace PlotDigitizer.Core
{
	public class AxisLocationNode : UpdatableNode<RectangleD>
	{
		private readonly InputImageNode inputImage;

		public AxisLocationNode(InputImageNode inputImage)
        {
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate()) {
				return;
			}
			Value = default;
			base.Update();
		}
	}
}
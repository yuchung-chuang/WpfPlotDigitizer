using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class InputImageNode : UpdatableNode<Image<Rgba, byte>>
	{
		public override bool IsUpdated => Value != null;

		public override void Update()
		{
			if (!IsUpdated)
				return;
			base.Update();
		}
	}
}
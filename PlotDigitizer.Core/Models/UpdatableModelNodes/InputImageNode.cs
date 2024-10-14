using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class InputImageNode : UpdatableNode<Image<Rgba, byte>>
	{
		public override bool IsUpdated => Data != null;
	}
}
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilterMinNode : UpdatableNode<Rgba>
	{
		public FilterMinNode(InputImageNode inputImage)
		{
			Data = new Rgba(0, 0, 0, byte.MaxValue);
			IsUpdated = true;
			DependsOn(inputImage);
        }
		protected override void Update()
		{
			if (!IsAllDependenciesUpdated())
				return;
			Data = new Rgba(0, 0, 0, byte.MaxValue);
			OnUpdated();
		}
	}
}
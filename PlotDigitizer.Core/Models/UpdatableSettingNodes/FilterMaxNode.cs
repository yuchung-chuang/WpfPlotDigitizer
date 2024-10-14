using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilterMaxNode : UpdatableNode<Rgba>
	{
		public FilterMaxNode(InputImageNode inputImage)
		{
			Data = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
			IsUpdated = true;
			DependsOn(inputImage);
        }

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated()) 
				return;
			Data = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
			OnUpdated();
		}
	}
}
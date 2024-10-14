namespace PlotDigitizer.Core
{
	public class DataTypeNode : UpdatableNode<DataType>
	{
		public DataTypeNode(InputImageNode inputImage)
		{
			DependsOn(inputImage);
        }

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated()) 
				return;
			Data = DataType.Continuous;
		}
	}
}
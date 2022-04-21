namespace PlotDigitizer.Core
{
	public class DataTypeNode : ModelNode<DataType>
	{
		public DataTypeNode()
		{
			Value = DataType.Continuous;
			IsUpdated = true;
		}
	}
}
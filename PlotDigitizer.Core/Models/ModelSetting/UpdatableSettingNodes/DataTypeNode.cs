namespace PlotDigitizer.Core
{
	public class DataTypeNode : UpdatableNode<DataType>
	{
		public DataTypeNode()
		{
			Value = DataType.Continuous;
			IsUpdated = true;
		}
	}
}
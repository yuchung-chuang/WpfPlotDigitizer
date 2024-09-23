namespace PlotDigitizer.Core
{
	public class DataTypeNode : UpdatableNode<DataType>
	{
		private readonly InputImageNode inputImage;

		public DataTypeNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate()) 
				return;
			Value = DataType.Continuous;
			base.Update();
		}
	}
}
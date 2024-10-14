using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class DataPointsNode : UpdatableNode<IEnumerable<PointD>>
	{
		private readonly EdittedImageNode edittedImage;
		private readonly DataTypeNode dataType;
		private readonly Dictionary<DataType, Func<Image<Rgba, byte>, IEnumerable<PointD>>> getPointsMethods;

		public DataPointsNode(EdittedImageNode edittedImage, 
			DataTypeNode dataType,
			IImageService imageService)
		{
			this.edittedImage = edittedImage;
			this.dataType = dataType;
			DependsOn(edittedImage);
			DependsOn(dataType);

			getPointsMethods = [];
			getPointsMethods.Add(DataType.Discrete, imageService.GetDiscretePoints);
			getPointsMethods.Add(DataType.Continuous, imageService.GetContinuousPoints);
		}

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated())
				return;

			Data = getPointsMethods[dataType.Data](edittedImage.Data);
            
			OnUpdated();
		}
	}
}
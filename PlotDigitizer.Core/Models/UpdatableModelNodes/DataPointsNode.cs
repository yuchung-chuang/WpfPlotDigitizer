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
			edittedImage.Updated += (s, e) => OnOutdated();
			edittedImage.Outdated += (s, e) => OnOutdated();
			dataType.Updated += (s, e) => OnOutdated();
			dataType.Outdated += (s, e) => OnOutdated();

			getPointsMethods = [];
			getPointsMethods.Add(DataType.Discrete, imageService.GetDiscretePoints);
			getPointsMethods.Add(DataType.Continuous, imageService.GetContinuousPoints);
		}

		public override void Update()
		{
			if (!edittedImage.CheckUpdate() || !dataType.CheckUpdate())
				return;

			Value = getPointsMethods[dataType.Value](edittedImage.Value);
            
			base.Update();
		}
	}
}
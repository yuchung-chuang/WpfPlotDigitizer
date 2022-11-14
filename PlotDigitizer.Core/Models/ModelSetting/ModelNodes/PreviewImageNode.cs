using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class PreviewImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly EdittedImageNode edittedImage;
		private readonly DataTypeNode dataType;

		public IEnumerable<PointD> Points { get; private set; }

		public override bool IsUpdated
		{
			get => base.IsUpdated;
			set => base.IsUpdated = value;
		}

		public PreviewImageNode(EdittedImageNode edittedImage, DataTypeNode dataType)
		{
			this.edittedImage = edittedImage;
			this.dataType = dataType;
			edittedImage.Updated += (s, e) => OnOutdated();
			edittedImage.Outdated += (s, e) => OnOutdated();
			dataType.Updated += (s, e) => OnOutdated();
			dataType.Outdated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!edittedImage.CheckUpdate() || !dataType.CheckUpdate())
				return;
			Value = edittedImage.Value.Copy();
			Points = dataType.Value switch
			{
				DataType.Discrete => Methods.GetDiscretePoints(Value),
				DataType.Continuous => Methods.GetContinuousPoints(Value),
				_ => throw new NotImplementedException(),
			};
			base.Update();
		}
	}
}
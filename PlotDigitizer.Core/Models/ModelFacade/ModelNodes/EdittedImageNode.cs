﻿using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class EdittedImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly FilteredImageNode filteredImage;

		public EdittedImageNode(FilteredImageNode filteredImage)
		{
			this.filteredImage = filteredImage;
			filteredImage.Updated += DependencyUpdated;
		}

		public override void Update()
		{
			if (!filteredImage.CheckUpdate())
				return;
			Value = filteredImage.Value.Copy();
			OnUpdated();
		}
	}
}
using Microsoft.AspNetCore.Http;

using PlotDigitizer.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Models
{
	public class Model : ModelFacade
	{
		public string InputImageSource { get; set; }

		public Model(InputImageNode inputImage, CroppedImageNode croppedImage, FilteredImageNode filteredImage, EdittedImageNode edittedImage, PreviewImageNode previewImage, DataNode data) : base(inputImage, croppedImage, filteredImage, edittedImage, previewImage, data)
		{
		}
	}
}

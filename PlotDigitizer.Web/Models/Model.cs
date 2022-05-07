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
		public double DisplayWidth { get; } = 500d;
		public Setting Setting { get; }
		public string InputImageSource => InputImage?.ToImgSrc();

		public string CroppedImageSource => CroppedImage?.ToImgSrc();

		public string FilteredImageSource => FilteredImage?.ToImgSrc();

		public string EdittedImageSource => EdittedImage?.ToImgSrc();


		public Model(InputImageNode inputImage, CroppedImageNode croppedImage, FilteredImageNode filteredImage, EdittedImageNode edittedImage, PreviewImageNode previewImage, DataNode data, Setting setting) : base(inputImage, croppedImage, filteredImage, edittedImage, previewImage, data)
		{
			Setting = setting;
		}
	}
}

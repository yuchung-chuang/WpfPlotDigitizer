using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlotDigitizer.Web.Models
{
	public static class ImageCaster
	{
		public static string ToImgSrc(this Image<Rgba,byte> image)
		{
			var bytes = CvInvoke.Imencode(".png", image);
			var base64 = Convert.ToBase64String(bytes);
			return $"data:image/png;base64,{base64}";
		}
		public static async Task<Image<Rgba, byte>> ToImageAsync(this IFormFile formFile)
		{
			if (formFile is null) {
				return null;
			}
			using var memoryStream = new MemoryStream();
			await formFile.CopyToAsync(memoryStream);
			return (Image.FromStream(memoryStream) as Bitmap).ToImage<Rgba, byte>();
		}
		public static async Task<string> ToImgSrcAsync(this IFormFile formFile)
		{
			if (formFile is null) {
				return null;
			}
			using var memoryStream = new MemoryStream();
			await formFile.CopyToAsync(memoryStream);
			var bytes = memoryStream.ToArray();
			var base64 = Convert.ToBase64String(bytes);
			new FileExtensionContentTypeProvider().TryGetContentType(formFile.FileName, out var mimeType);
			mimeType ??= "application/octet-stream";
			return $"data:{mimeType};base64,{base64}";
		}

	}
}

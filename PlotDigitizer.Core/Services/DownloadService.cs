using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace PlotDigitizer.Core
{
    public class DownloadService(ILogger<DownloadService> logger,
        IMessageBoxService messageBox) : IDownloadService, IDisposable
    {
        private readonly HttpClient httpClient = new();

        public async Task<Image<Rgba, byte>> DownloadImageAsync(Uri url, CancellationToken token)
        {
            var filePath = Path.GetTempFileName();
            try {
                logger?.LogDebug($"Starting download of image from URL: {url}");

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                var assembly = Assembly.GetExecutingAssembly().GetName();
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue(assembly.Name, assembly.Version.ToString()));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));

                using var response = await httpClient.SendAsync(request, token);
                response.EnsureSuccessStatusCode();

                if (!response.Content.Headers.ContentType.MediaType.StartsWith("image/")) {
                    throw new HttpRequestException("The url does not point to an image, it may point to a web page that contains the image. Make sure you supply the url that points to the raw image.");
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var bitmap = Image.FromStream(stream) as Bitmap;

                logger?.LogInformation($"Successfully downloaded image from URL: {url}");
                return bitmap?.ToImage<Rgba, byte>();
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Error occurred while downloading image from URL: {e.Url}");
                messageBox.Show_OK(ex.Message, "Warning");
                return null;
            }
            finally {
                File.Delete(filePath);
                logger?.LogDebug($"Temporary file deleted: {filePath}");
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            httpClient.Dispose();
        }
    }
}

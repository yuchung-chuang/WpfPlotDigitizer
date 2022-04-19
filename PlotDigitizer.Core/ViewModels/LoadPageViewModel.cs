using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Core
{
	public class LoadPageViewModel : PageViewModelBase
	{
		private readonly IFileDialogService fileDialogService;
		private readonly IAwaitTaskService awaitTaskService;
		private readonly IClipboard clipboard;
		private readonly IMessageBoxService messageBox;

		public event EventHandler NextPage;

		public RelayCommand BrowseCommand { get; private set; }
		public RelayCommand PasteCommand { get; private set; }
		public RelayCommand<DropEventArgs> DropCommand { get; private set; }

		public Model Model { get; }

		public LoadPageViewModel()
		{
			Name = "LoadPage";
			BrowseCommand = new RelayCommand(Browse);
			PasteCommand = new RelayCommand(Paste);
			DropCommand = new RelayCommand<DropEventArgs>(Drop);
		}

		public LoadPageViewModel(Model model,
			IFileDialogService fileDialogService,
			IAwaitTaskService awaitTaskService,
			IClipboard clipboard,
			IMessageBoxService messageBox) : this()
		{
			Model = model;
			this.fileDialogService = fileDialogService;
			this.awaitTaskService = awaitTaskService;
			this.clipboard = clipboard;
			this.messageBox = messageBox;
		}

		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}

		private void Paste()
		{
			if (clipboard.ContainsImage()) {
				SetModelImage(clipboard.GetImage());
			} else if (clipboard.ContainsFileDropList()) {
				SetModelImage(new Image<Rgba, byte>(clipboard.GetFileDropList()[0]));
			} else {
				messageBox.Show_OK("Clipboard does not contain image.", "Warning");
				return;
			}
		}

		public void SetModelImage(Image<Rgba, byte> image)
		{
			Model.InputImage = image;
			OnNextPage();
		}

		private void Browse()
		{
			var filter = "Images (*.jpg;*.jpeg;*.png;*.bmp;*.tif) |*.jpg;*.jpeg;*.png;*.bmp;*.tif|" +
				"(*.jpg;*.jpeg) |*.jpg;*.jpeg|" +
				"(*.png) |*.png|" +
				"(*.bmp) |*.bmp|" +
				"(*.tif) |*.tif|" +
				"Any |*.*";
			var result = fileDialogService.OpenFileDialog(filter);
			if (!result.IsValid) {
				return;
			}
			SetModelImage(new Image<Rgba, byte>(result.FileName));
		}

		private async void Drop(DropEventArgs e)
		{
			if (e.Type == DropEventArgs.DropType.File) {
				SetModelImage(new Image<Rgba, byte>(e.FileName));
			} else if (e.Type == DropEventArgs.DropType.Url) {
				// make sure it returns to the main thread
				var image = await awaitTaskService.RunAsync(DownloadImage).ConfigureAwait(true);
				// as this line updates the view
				SetModelImage(image); 
			}

			async Task<Image<Rgba, byte>> DownloadImage(CancellationToken token)
			{
				var filePath = Path.GetTempFileName();
				try {
					// set up http client
					using var httpClient = new HttpClient();
					// form a request, it's important to attach user-agent header to the request, otherwise the server may return "403 forbidden"
					var request = new HttpRequestMessage(HttpMethod.Get, e.Url);
					var assembly = Assembly.GetExecutingAssembly().GetName();
					request.Headers.UserAgent.Add(new ProductInfoHeaderValue(assembly.Name, assembly.Version.ToString()));
					request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
					// send the request, wait for the response
					var response = await httpClient.SendAsync(request);
					// ensure it returns with a success code "200"
					response.EnsureSuccessStatusCode();
					// read the content as byte[]
					var data = await response.Content.ReadAsByteArrayAsync();
					// save the content to a temperary file
					using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
					await fileStream.WriteAsync(data, 0, data.Length);
					// load image from the file
					return new Image<Rgba, byte>(filePath);
				}
				catch (Exception ex) {
					messageBox.Show_OK(ex.Message, "Warning");
					return null;
				}
				finally {
					// delete the temperary file afterward
					File.Delete(filePath);
				}

			}
		}

	}
}

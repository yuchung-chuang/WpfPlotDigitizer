using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using PropertyChanged;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Core
{
    public class LoadPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly IAwaitTaskService awaitTaskService;
        private readonly IClipboardService clipboard;
        private readonly IFileDialogService fileDialogService;
        private readonly IMessageBoxService messageBox;
        private readonly ILogger<LoadPageViewModel> logger;
        private readonly IPageService pageService;

        #endregion Fields

        #region Properties
        [OnChangedMethod(nameof(OnFilePathChanged))]
        public string FilePath { get; set; }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand<DropEventArgs> DropCommand { get; private set; }
        public Model Model { get; }
        public RelayCommand PasteCommand { get; private set; }

        #endregion Properties

        #region Constructors

        public LoadPageViewModel()
        {
            Name = "Load Page";
            BrowseCommand = new RelayCommand(Browse);
            PasteCommand = new RelayCommand(Paste);
            DropCommand = new RelayCommand<DropEventArgs>(Drop);
        }

        public LoadPageViewModel(Model model,
            IFileDialogService fileDialogService,
            IAwaitTaskService awaitTaskService,
            IClipboardService clipboard,
            IMessageBoxService messageBox,
            ILogger<LoadPageViewModel> logger,
            IPageService pageService) : this()
        {
            Model = model;
            this.fileDialogService = fileDialogService;
            this.awaitTaskService = awaitTaskService;
            this.clipboard = clipboard;
            this.messageBox = messageBox;
            this.logger = logger;
            this.pageService = pageService;
        }

        #endregion Constructors

        #region Methods

        private void SetModelImage(Image<Rgba, byte> image)
        {
            if (image == null) {
                logger?.LogWarning($"Attempted to set a null image to the model.");
                return;
            }

            Model.InputImage = image;
            logger?.LogInformation($"Image set successfully in the model.");

            // Automatically go to the next page when the image is loaded.
            if (pageService.NextPageCommand.CanExecute(null)) {
                pageService.NextPageCommand.Execute(null);
                logger?.LogInformation($"Navigated to the next page after image load.");
            }
        }

        private void Browse()
        {
            logger?.LogInformation($"Browse command invoked.");

            var filter = "Images (*.jpg;*.jpeg;*.png;*.bmp;*.tif) |*.jpg;*.jpeg;*.png;*.bmp;*.tif|" +
                "(*.jpg;*.jpeg) |*.jpg;*.jpeg|" +
                "(*.png) |*.png|" +
                "(*.bmp) |*.bmp|" +
                "(*.tif) |*.tif|" +
                "Any |*.*";
            var result = fileDialogService.OpenFileDialog(filter);

            if (!result.IsValid) {
                logger?.LogWarning($"Browse command cancelled by the user or invalid file selection.");
                return;
            }

            logger?.LogInformation($"File selected for browsing: {result.FileName}");
            SetModelImage(new Image<Rgba, byte>(result.FileName));
        }

        private async void Drop(DropEventArgs e)
        {
            logger?.LogInformation($"Drop command invoked with drop type: {e.Type}");

            if (e.Type == DropEventArgs.DropType.File) {
                logger?.LogInformation($"File dropped: {e.FileName}");
                SetModelImage(new Image<Rgba, byte>(e.FileName));
            }
            else if (e.Type == DropEventArgs.DropType.Url) {
                logger?.LogInformation($"URL dropped: {e.Url}");
                var image = await awaitTaskService.RunAsync(DownloadImage).ConfigureAwait(true);
                SetModelImage(image);
            }

            async Task<Image<Rgba, byte>> DownloadImage(CancellationToken token)
            {
                var filePath = Path.GetTempFileName();
                try {
                    logger?.LogDebug($"Starting download of image from URL: {e.Url}");

                    using var httpClient = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, e.Url);
                    var assembly = Assembly.GetExecutingAssembly().GetName();
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue(assembly.Name, assembly.Version.ToString()));
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();

                    logger?.LogInformation($"Successfully downloaded image from URL: {e.Url}");
                    return (Image.FromStream(stream) as Bitmap).ToImage<Rgba, byte>();
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
        }

        private void Paste()
        {
            logger?.LogInformation($"Paste command invoked.");

            if (clipboard.ContainsImage()) {
                logger?.LogInformation($"Image found in clipboard.");
                SetModelImage(clipboard.GetImage());
            }
            else if (clipboard.ContainsFileDropList()) {
                var filePath = clipboard.GetFileDropList()[0];
                logger?.LogInformation($"File found in clipboard: {filePath}");
                SetModelImage(new Image<Rgba, byte>(filePath));
            }
            else {
                logger?.LogWarning($"Clipboard does not contain a valid image or file.");
                messageBox.Show_OK("Clipboard does not contain image.", "Warning");
            }
        }

        public void OnFilePathChanged()
        {
            logger?.LogInformation($"File path changed: {FilePath}");

            if (!File.Exists(FilePath)) {
                logger?.LogWarning($"File path does not exist: {FilePath}");
                return;
            }

            SetModelImage(new Image<Rgba, byte>(FilePath));
            logger?.LogInformation($"Image successfully loaded from file path: {FilePath}");
        }

        #endregion Methods
    }
}

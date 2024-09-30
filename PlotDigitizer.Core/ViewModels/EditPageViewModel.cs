using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
    public class EditPageViewModel : ViewModelBase, IDisposable
    {
        private readonly IImageService imageService;
        private readonly ILogger<EditPageViewModel> logger;

        #region Properties
        public IEditService<Image<Rgba, byte>> EditService { get; set; }
        public Image<Rgba, byte> Image { get; set; }
        public Model Model { get; private set; }
        public bool IsEnabled => Model != null && Model.FilteredImage != null;

        public IEnumerable<string> RedoList
        {
            get {
                if (!EditService.IsInitialised)
                    yield break;
                for (int i = EditService.Index; i < EditService.TagList.Count; i++) {
                    yield return EditService.TagList[i];
                }
            }
        }

        public IEnumerable<string> UndoList
        {
            get {
                if (!EditService.IsInitialised)
                    yield break;
                for (var i = EditService.Index; i >= 0; i--) {
                    yield return EditService.TagList[i];
                }
            }
        }

        public RelayCommand<int> RedoToCommand { get; private set; }
        public RelayCommand<int> UndoToCommand { get; private set; }
        public RelayCommand<(Image<Rgba, byte> obj, string tag)> EditCommand { get; private set; }
        public RelayCommand<int> GoToCommand { get; private set; }
        public RelayCommand RedoCommand { get; private set; }
        public RelayCommand UndoCommand { get; private set; }
        public RelayCommand ClearBorderCommand { get; set; }

        #endregion Properties

        #region Constructors

        public EditPageViewModel()
        {
            Name = "Edit Page";
            InitializeCommands();
        }

        public EditPageViewModel(Model model,
            IImageService imageService,
            IEditService<Image<Rgba, byte>> editService,
            ILogger<EditPageViewModel> logger) : this()
        {
            Model = model;
            this.imageService = imageService;
            EditService = editService;
            this.logger = logger;

            logger?.LogInformation("EditPageViewModel initialized with Model and services.");
        }

        private void InitializeCommands()
        {
            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);
            GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
            EditCommand = new RelayCommand<(Image<Rgba, byte>, string)>(Edit, CanEdit);
            UndoToCommand = new RelayCommand<int>(UndoTo);
            RedoToCommand = new RelayCommand<int>(RedoTo);
            ClearBorderCommand = new RelayCommand(ClearBorder);
        }

        #endregion Constructors

        #region Methods
        public override void Enter()
        {
            base.Enter();
            logger?.LogInformation($"Entered EditPageViewModel with IsEnabled status: {IsEnabled}");

            if (!IsEnabled) {
                logger?.LogWarning("EditPageViewModel is not enabled. Model or FilteredImage is null.");
                return;
            }

            if (EditService.IsInitialised) {
                logger?.LogInformation("EditService is already initialized. Skipping reinitialization.");
                return;
            }

            logger?.LogInformation("Initializing EditService with the filtered image.");
            EditService.Initialise(Model.FilteredImage);
            EditService.PropertyChanged += EditService_PropertyChanged;
            logger?.LogInformation("EditService initialized successfully.");
        }

        public override void Leave()
        {
            base.Leave();
            logger?.LogInformation("Leaving EditPageViewModel.");

            if (!IsEnabled) {
                logger?.LogWarning("EditPageViewModel is not enabled. Model or FilteredImage is null.");
                return;
            }

            Model.EdittedImage = Image;
            logger?.LogInformation("Set the edited image to the model.");
        }

        private bool CanEdit((Image<Rgba, byte> obj, string tag) arg) => EditService.CanEdit(arg);
        private bool CanGoTo(int targetIndex) => EditService.CanGoTo(targetIndex);
        private bool CanRedo() => EditService.CanRedo();
        private bool CanUndo() => EditService.CanUndo();

        private void Edit((Image<Rgba, byte> obj, string tag) edit)
        {
            logger?.LogInformation($"Applying edit with tag: {edit.tag}");
            EditService.Edit(edit);
        }

        private void GoTo(int targetIndex)
        {
            logger?.LogInformation($"Navigating to index: {targetIndex}");
            EditService.GoTo(targetIndex);
        }

        private void Redo()
        {
            logger?.LogInformation("Redoing the last action.");
            EditService.Redo();
        }

        private void Undo()
        {
            logger?.LogInformation("Undoing the last action.");
            EditService.Undo();
        }

        private void RedoTo(int index)
        {
            if (index <= 0)
                return;

            var targetIndex = EditService.Index + index;
            logger?.LogInformation($"Redoing to index: {targetIndex}");

            if (EditService.CanGoTo(targetIndex))
                EditService.GoTo(targetIndex);
        }

        private void UndoTo(int index)
        {
            if (index <= 0)
                return;

            var targetIndex = EditService.Index - index;
            logger?.LogInformation($"Undoing to index: {targetIndex}");

            if (EditService.CanGoTo(targetIndex))
                EditService.GoTo(targetIndex);
        }

        private void ClearBorder()
        {
            logger?.LogInformation("Clearing border from the current image.");
            var image = imageService.ClearBorder(EditService.CurrentObject);
            Edit((image, "Clear Border"));
        }

        private void EditService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            logger?.LogInformation($"EditService property changed: {e.PropertyName}");

            if (e.PropertyName == nameof(EditService.Index)) {
                OnPropertyChanged(nameof(UndoList));
                OnPropertyChanged(nameof(RedoList));
                UndoCommand.RaiseCanExecuteChanged();
                RedoCommand.RaiseCanExecuteChanged();
            }
        }

        public void Dispose()
        {
            logger?.LogInformation("Disposing EditPageViewModel.");
            EditService.PropertyChanged -= EditService_PropertyChanged;
            GC.SuppressFinalize(this);
            logger?.LogInformation("EditPageViewModel disposed.");
        }

        #endregion Methods
    }
}

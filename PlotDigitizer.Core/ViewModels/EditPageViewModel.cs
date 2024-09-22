using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace PlotDigitizer.Core
{
	// TODO: Clear Border checkbox, allowing user to decide.
	public class EditPageViewModel : PageViewModelBase, IDisposable
	{
		private readonly IImageService imageService;
		#region Properties

		public EditManager<Image<Rgba, byte>> EditManager { get; set; }
		public Image<Rgba, byte> Image { get; set; }
		public Model Model { get; private set; }
		public bool IsEnabled => Model != null && Model.FilteredImage != null;
		public IEnumerable<string> RedoList
		{
			get {
				if (!EditManager.IsInitialised)
					yield break;
				for (int i = EditManager.Index; i < EditManager.TagList.Count; i++) {
					yield return EditManager.TagList[i];
				}
			}
		}

		public IEnumerable<string> UndoList
		{
			get {
				if (!EditManager.IsInitialised)
					yield break;
				for (var i = EditManager.Index; i >= 0; i--) {
					yield return EditManager.TagList[i];
				}
			}
		}

		public RelayCommand<int> RedoToCommand { get; private set; }
		public RelayCommand<int> UndoToCommand { get; private set; }
		public RelayCommand<(Image<Rgba, byte> obj, string tag)> EditCommand { get; private set; }
		public RelayCommand<int> GoToCommand { get; private set; }
		public RelayCommand RedoCommand { get; private set; }
		public RelayCommand UndoCommand { get; private set; }

		#endregion Properties

		#region Constructors

		public EditPageViewModel()
		{
			Name = "EditPage";
			UndoCommand = new RelayCommand(Undo, CanUndo);
			RedoCommand = new RelayCommand(Redo, CanRedo);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
			EditCommand = new RelayCommand<(Image<Rgba, byte>, string)>(Edit, CanEdit);
			UndoToCommand = new RelayCommand<int>(UndoTo);
			RedoToCommand = new RelayCommand<int>(RedoTo);
			EditManager = new EditManager<Image<Rgba, byte>>();
		}

		public EditPageViewModel(Model model,
			IImageService imageService) : this()
		{
			Model = model;
			this.imageService = imageService;
		}
		#endregion Constructors

		#region Methods
		public override void Enter()
		{
			if (!IsEnabled) 
				return;
			if (EditManager.IsInitialised) 
				return;
			
			base.Enter();
			var image = imageService.ClearBorder(Model.FilteredImage);
			EditManager.Initialise(image);
			EditManager.PropertyChanged += EditManager_PropertyChanged;
			EditManager.ObjectList.CollectionChanged += ObjectList_CollectionChanged;
		}

		public override void Leave()
		{
			if (!IsEnabled) {
				return;
			}
			base.Leave();
			Model.EdittedImage = Image;
		}
		private bool CanEdit((Image<Rgba, byte> obj, string tag) arg) => EditManager.CanEdit(arg);
		private bool CanGoTo(int targetIndex) => EditManager.CanGoTo(targetIndex);
		private bool CanRedo() => EditManager.CanRedo();
		private bool CanUndo() => EditManager.CanUndo();
		private void Edit((Image<Rgba, byte> obj, string tag) edit) => EditManager.Edit(edit);
		private void GoTo(int targetIndex) => EditManager.GoTo(targetIndex);
		private void Redo() => EditManager.Redo();
		private void Undo() => EditManager.Undo();

		private void RedoTo(int index)
		{
			if (index <= 0)
				return;
			var targetIndex = EditManager.Index + index;
			if (EditManager.CanGoTo(targetIndex))
				EditManager.GoTo(targetIndex);
		}

		private void UndoTo(int index)
		{
			if (index <= 0)
				return;
			var targetIndex = EditManager.Index - index;
			if (EditManager.CanGoTo(targetIndex))
				EditManager.GoTo(targetIndex);
		}

		private void EditManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(EditManager.Index)) {
				OnPropertyChanged(nameof(UndoList));
				OnPropertyChanged(nameof(RedoList));
				UndoCommand.RaiseCanExecuteChanged();
				RedoCommand.RaiseCanExecuteChanged();
			}
		}
		private void ObjectList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		}

		public void Dispose()
		{
			EditManager.PropertyChanged -= EditManager_PropertyChanged;
			if (EditManager.ObjectList is not null)
				EditManager.ObjectList.CollectionChanged -= ObjectList_CollectionChanged;
			GC.SuppressFinalize(this);
		}

		#endregion Methods
	}
}
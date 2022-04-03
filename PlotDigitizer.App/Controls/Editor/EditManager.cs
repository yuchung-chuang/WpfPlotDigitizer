
using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.App
{
	public class EditManager<TObject> : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public int Index { get; private set; }
		public List<TObject> ObjectList { get; private set; }

		public TObject CurrentObject => ObjectList[Index];

		public List<string> TagList { get; private set; }

		public string CurrentTag => TagList[Index];

		public RelayCommand UndoCommand { get; private set; }

		public RelayCommand RedoCommand { get; private set; }

		public RelayCommand<int> GoToCommand { get; private set; }

		public RelayCommand<(TObject obj, string tag)> EditCommand { get; set; }

		public EditManager(TObject _object) : this()
		{
			Initialise(_object);
		}

		public EditManager()
		{
			UndoCommand = new RelayCommand(Undo, CanUndo);
			RedoCommand = new RelayCommand(Redo, CanRedo);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
			EditCommand = new RelayCommand<(TObject, string)>(Edit, CanEdit);
		}

		public void Initialise(TObject _object)
		{
			ObjectList = new List<TObject>
			{
				_object,
			};
			TagList = new List<string>
			{
				"initialise",
			};
			Index = 0;
		}

		protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		protected virtual bool CanEdit((TObject obj, string tag) arg) => true;

		private void Undo() => Index--;

		private bool CanUndo() => Index > 0;

		private void Redo() => Index++;

		private bool CanRedo() => Index < ObjectList.Count - 1;

		private void GoTo(int targetIndex) => Index = targetIndex;

		private bool CanGoTo(int targetIndex) => targetIndex >= 0 && targetIndex < TagList.Count;

		private void Edit((TObject obj, string tag) edit)
		{
			ObjectList.RemoveRange(Index + 1, ObjectList.Count - Index - 1);
			ObjectList.Add(edit.obj);
			OnPropertyChanged(nameof(ObjectList));

			TagList.RemoveRange(Index + 1, TagList.Count - Index - 1);
			TagList.Add(edit.tag);
			OnPropertyChanged(nameof(TagList));

			Index++;
		}
	}
}
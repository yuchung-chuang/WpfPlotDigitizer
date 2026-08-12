using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IEditService{TObject}"/>. Records the calls a view model
	/// makes and lets each guard be forced, so the view model can be tested without the real
	/// undo stack.
	/// </summary>
	internal sealed class FakeEditService<TObject> : IEditService<TObject>
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public TObject CurrentObject { get; set; }

		public string CurrentTag { get; set; }

		public int Index { get; set; }

		public bool IsInitialised { get; set; }

		public IList<TObject> ObjectList { get; set; } = [];

		public IList<string> TagList { get; set; } = [];

		public bool CanEditResult { get; set; } = true;

		public bool CanGoToResult { get; set; } = true;

		public bool CanRedoResult { get; set; } = true;

		public bool CanUndoResult { get; set; } = true;

		public List<TObject> InitialisedWith { get; } = [];

		public List<(TObject obj, string tag)> Edits { get; } = [];

		public List<int> GoToTargets { get; } = [];

		public int RedoCallCount { get; private set; }

		public int UndoCallCount { get; private set; }

		public void Initialise(TObject _object)
		{
			InitialisedWith.Add(_object);
			IsInitialised = true;
			CurrentObject = _object;
			ObjectList = [_object];
			TagList = ["initialise"];
			Index = 0;
		}

		public bool CanEdit((TObject obj, string tag) arg) => CanEditResult;

		public bool CanGoTo(int targetIndex) => CanGoToResult;

		public bool CanRedo() => CanRedoResult;

		public bool CanUndo() => CanUndoResult;

		public void Edit((TObject obj, string tag) edit)
		{
			Edits.Add(edit);
			ObjectList.Add(edit.obj);
			TagList.Add(edit.tag);
			Index++;
			RaisePropertyChanged(nameof(Index));
		}

		public void GoTo(int targetIndex)
		{
			GoToTargets.Add(targetIndex);
			Index = targetIndex;
			RaisePropertyChanged(nameof(Index));
		}

		public void Redo()
		{
			RedoCallCount++;
			Index++;
			RaisePropertyChanged(nameof(Index));
		}

		public void Undo()
		{
			UndoCallCount++;
			Index--;
			RaisePropertyChanged(nameof(Index));
		}

		public void RaisePropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

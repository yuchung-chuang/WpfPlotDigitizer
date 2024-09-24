using System.Collections.Generic;
using System.ComponentModel;

namespace PlotDigitizer.Core
{
	public interface IEditService<TObject> : INotifyPropertyChanged
	{
		TObject CurrentObject { get; }
		string CurrentTag { get; }
		int Index { get; }
		bool IsInitialised { get; }
		IList<TObject> ObjectList { get; }
		IList<string> TagList { get; }
		void Initialise(TObject _object);

		bool CanEdit((TObject obj, string tag) arg);
		bool CanGoTo(int targetIndex);
		bool CanRedo();
		bool CanUndo();
		void Edit((TObject obj, string tag) edit);
		void GoTo(int targetIndex);
		void Redo();
		void Undo();
	}
}
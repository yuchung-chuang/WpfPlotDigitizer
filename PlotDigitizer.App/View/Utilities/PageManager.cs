using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public class PageManager : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Public read/write access to allow two-way binding 
		/// </summary>
		public int PageIndex { get; set; } = 0; 

		public List<Page> PageList { get; private set; }

		public Page CurrentPage => PageList[PageIndex];

		public RelayCommand BackCommand { get; private set; }

		public RelayCommand NextCommand { get; private set; }

		public RelayCommand<int> GoToCommand { get; private set; }

		public RelayCommand<Type> GoToByTypeCommand { get; private set; }

		private PageManager()
		{
			BackCommand = new RelayCommand(GoBack, CanGoBack);
			NextCommand = new RelayCommand(GoNext, CanGoNext);
			GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
			GoToByTypeCommand = new RelayCommand<Type>(GoTo, CanGoTo);
		}

		public PageManager(List<Page> pageList) : this()
		{
			PageList = pageList;
		}

		public Page GetPage(Type TPage)
		{
			return PageList.FirstOrDefault(p => p.GetType() == TPage);
		}

		public TPage GetPage<TPage>() where TPage : class
		{
			return PageList.FirstOrDefault(p => p is TPage) as TPage;
		}

		private void GoBack()
		{
			PageIndex--;
			BackCommand.RaiseCanExecuteChanged();
			NextCommand.RaiseCanExecuteChanged();
		}

		private bool CanGoBack()
		{
			return PageIndex > 0;
		}

		private void GoNext()
		{
			PageIndex++;
			BackCommand.RaiseCanExecuteChanged();
			NextCommand.RaiseCanExecuteChanged();
		}

		private bool CanGoNext()
		{
			return PageIndex < PageList.Count - 1;
		}

		private void GoTo(int targetIndex)
		{
			PageIndex = targetIndex;
		}

		private bool CanGoTo(int targetIndex)
		{
			return targetIndex >= 0 && targetIndex < PageList.Count;
		}

		private void GoTo(Type TPage)
		{
			var index = PageList.FindIndex(p => p.GetType() == TPage);
			PageIndex = index;
		}

		private bool CanGoTo(Type TPage)
		{
			return PageList.Any(p => p.GetType() == TPage);
		}
	}
}
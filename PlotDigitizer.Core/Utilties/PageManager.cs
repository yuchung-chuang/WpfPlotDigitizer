using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlotDigitizer.Core
{
    [AddINotifyPropertyChangedInterface]
    public class PageManager
    {
        /// <summary>
        /// Public read/write access to allow two-way binding 
        /// </summary>
        [OnChangedMethod(nameof(OnPageIndexChanged))]
        public int PageIndex { get; set; } = 0;

        [OnChangedMethod(nameof(OnPageTypeListChanged))]
        public List<PageViewModelBase> PageList { get; private set; }
        public PageViewModelBase CurrentPage => PageList[PageIndex];

        public RelayCommand BackCommand { get; private set; }
        public RelayCommand NextCommand { get; private set; }
        public RelayCommand<int> GoToCommand { get; private set; }
        public RelayCommand<Type> GoToByTypeCommand { get; private set; }

        public PageManager()
        {
            BackCommand = new RelayCommand(GoBack, CanGoBack);
            NextCommand = new RelayCommand(GoNext, CanGoNext);
            GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
            GoToByTypeCommand = new RelayCommand<Type>(GoTo, CanGoTo);
        }

        public void Initialise(List<PageViewModelBase> pageList)
        {
            PageList = pageList;
            PageIndex = 0;
        }

        public PageViewModelBase GetPage(Type type) => PageList.FirstOrDefault(t => t.GetType() == type);

        public TPage GetPage<TPage>() where TPage : PageViewModelBase => PageList.FirstOrDefault(t => t.GetType() == typeof(TPage)) as TPage;

        private void GoBack() => PageIndex--;

        private bool CanGoBack() => PageIndex > 0;

        private void GoNext() => PageIndex++;

        private bool CanGoNext() => PageIndex < PageList.Count - 1;

        private void GoTo(int targetIndex) => PageIndex = targetIndex;

        private bool CanGoTo(int targetIndex) => targetIndex >= 0 && targetIndex < PageList.Count;

        private void GoTo(Type type) => PageIndex = PageList.FindIndex(t => t.GetType() == type);

        private bool CanGoTo(Type type) => PageList.Any(t => t.GetType() == type);
        private void OnPageIndexChanged(int oldIndex, int newIndex)
        {
            BackCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            PageList[oldIndex].Leave();
            PageList[newIndex].Enter();
        }
        private void OnPageTypeListChanged()
        {
            GoToCommand.RaiseCanExecuteChanged();
            GoToByTypeCommand.RaiseCanExecuteChanged();
        }
    }
}
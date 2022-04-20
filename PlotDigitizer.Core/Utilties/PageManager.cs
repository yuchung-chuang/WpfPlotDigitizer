using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PlotDigitizer.Core
{
    public class PageManager : INotifyPropertyChanged
    {
        private const int INITIAL_PAGE_INDEX = 0;
        private int browseIndex = 0;
        private readonly IList<int> browseHistory = new List<int>(10) { INITIAL_PAGE_INDEX };

        public event PropertyChangedEventHandler PropertyChanged;

        [OnChangedMethod(nameof(OnPageIndexChanged))]
        public int PageIndex { get; set; } = INITIAL_PAGE_INDEX;
        [OnChangedMethod(nameof(OnPageTypeListChanged))]
        public IList<PageViewModelBase> PageList { get; private set; }
        public PageViewModelBase CurrentPage => PageList[PageIndex];

        public RelayCommand PreviousPageCommand { get; private set; }
        public RelayCommand NextPageCommand { get; private set; }
        public RelayCommand<int> GoToCommand { get; private set; }
        public RelayCommand<Type> GoToByTypeCommand { get; private set; }
        public RelayCommand BrowseBackCommand { get; private set; }
        public RelayCommand BrowseForwardCommand { get; private set; }

        public PageManager()
        {
            PreviousPageCommand = new RelayCommand(PreviousPage, CanPreviousPage);
            NextPageCommand = new RelayCommand(NextPage, CanNextPage);
            GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
            GoToByTypeCommand = new RelayCommand<Type>(GoTo, CanGoTo);
            BrowseBackCommand = new RelayCommand(BrowseBack, CanBrowseBack);
            BrowseForwardCommand = new RelayCommand(BrowseForward, CanBrowseForward);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public void Initialise(IList<PageViewModelBase> pageList)
        {
            PageList = pageList;
            PageIndex = 0;
        }

        public PageViewModelBase GetPage(Type type) => PageList.FirstOrDefault(t => t.GetType() == type);

        public TPage GetPage<TPage>() where TPage : PageViewModelBase => PageList.FirstOrDefault(t => t.GetType() == typeof(TPage)) as TPage;
        private void BrowseBack() => PageIndex = browseHistory[--browseIndex];

        private bool CanBrowseBack() => browseIndex > 0;

        private void BrowseForward() => PageIndex = browseHistory[++browseIndex];

        private bool CanBrowseForward() => browseIndex < browseHistory.Count() - 1;

        private void PreviousPage()
        {
            PageIndex--;
            UpdateBrowseHistory();
        }

        private bool CanPreviousPage() => PageIndex > 0;

        private void NextPage()
        {
            PageIndex++;
            UpdateBrowseHistory();
        }

        private bool CanNextPage() => PageIndex < PageList.Count - 1;

        private void GoTo(int targetIndex)
        {
            PageIndex = targetIndex;
            UpdateBrowseHistory();
        }

        private bool CanGoTo(int targetIndex) => targetIndex >= 0 && targetIndex < PageList.Count && targetIndex != PageIndex;

        private void GoTo(Type type)
        {
            PageIndex = PageList.IndexOf(GetPage(type));
            UpdateBrowseHistory();
        }

        private bool CanGoTo(Type type) => PageList.Any(t => t.GetType() == type);
        private void OnPageIndexChanged(int oldIndex, int newIndex)
        {
            PreviousPageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
            PageList[oldIndex].Leave();
            PageList[newIndex].Enter();
        }
        private void UpdateBrowseHistory()
        {
            while (browseIndex != browseHistory.Count() - 1) {
                browseHistory.RemoveAt(browseHistory.Count() - 1);
            }
            browseHistory.Add(PageIndex);
            browseIndex++;

            if (browseHistory.Count() == 10) {
                browseHistory.RemoveAt(0);
            }
        }
        private void OnPageTypeListChanged()
        {
            GoToCommand.RaiseCanExecuteChanged();
            GoToByTypeCommand.RaiseCanExecuteChanged();
        }
    }
}
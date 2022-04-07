using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
    [AddINotifyPropertyChangedInterface]
    public class PageManager
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Public read/write access to allow two-way binding 
        /// </summary>
        public int PageIndex { get; set; } = 0;
        public List<Type> PageTypeList { get; private set; }

        public Page CurrentPage => serviceProvider.GetService(PageTypeList[PageIndex]) as Page;

        public RelayCommand BackCommand { get; private set; }
        public RelayCommand NextCommand { get; private set; }
        public RelayCommand<int> GoToCommand { get; private set; }
        public RelayCommand<Type> GoToByTypeCommand { get; private set; }

        public PageManager(IServiceProvider serviceProvider)
        {
            BackCommand = new RelayCommand(GoBack, CanGoBack);
            NextCommand = new RelayCommand(GoNext, CanGoNext);
            GoToCommand = new RelayCommand<int>(GoTo, CanGoTo);
            GoToByTypeCommand = new RelayCommand<Type>(GoTo, CanGoTo);
            this.serviceProvider = serviceProvider;
        }

        public void Initialise(List<Type> pageList)
        {
            PageTypeList = pageList;
            PageIndex = 0;
        }

        public Page GetPage(Type type) => serviceProvider.GetService(PageTypeList.FirstOrDefault(t => t == type)) as Page;

        public TPage GetPage<TPage>() where TPage : class => PageTypeList.FirstOrDefault(t => t == typeof(TPage)) as TPage;

        private void GoBack() => PageIndex--;

        private bool CanGoBack() => PageIndex > 0;

        private void GoNext() => PageIndex++;

        private bool CanGoNext() => PageIndex < PageTypeList.Count - 1;

        private void GoTo(int targetIndex) => PageIndex = targetIndex;

        private bool CanGoTo(int targetIndex) => targetIndex >= 0 && targetIndex < PageTypeList.Count;

        private void GoTo(Type type) => PageIndex = PageTypeList.FindIndex(t => t == type);

        private bool CanGoTo(Type type) => PageTypeList.Any(t => t == type);
    }
}
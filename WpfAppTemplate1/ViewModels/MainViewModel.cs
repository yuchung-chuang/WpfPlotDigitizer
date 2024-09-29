using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace WpfAppTemplate1
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDemoService demoService;

        public ICommand SubmitCommand { get; set; }
        public Rect Rect { get; set; } = new Rect(10, 10, 100, 100);
        public MainViewModel()
        {
            SubmitCommand = new RelayCommand(Submit);
        }

        public MainViewModel(IDemoService demoService) : this()
        {
            this.demoService = demoService;
        }

        private void Submit()
        {
            Debug.WriteLine("Submit button pressed.");
        }
    }
}
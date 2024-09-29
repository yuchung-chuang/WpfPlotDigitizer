namespace WpfAppTemplate1
{
    public interface IWindowService
    {
        void ShowWindow(ViewModelBase viewModel);

        bool CloseWindow(ViewModelBase viewModel);
        void ShowMainWindow(MainViewModel viewModel);
    }
}

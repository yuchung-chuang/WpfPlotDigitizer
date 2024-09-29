using PropertyChanged;
using System.ComponentModel;

namespace WpfAppTemplate1
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void Enter()
        {

        }

        public virtual void Leave()
        {

        }
    }
}
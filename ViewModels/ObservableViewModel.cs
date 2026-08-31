using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrystalLens.ViewModels
{
    public abstract class ObservableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

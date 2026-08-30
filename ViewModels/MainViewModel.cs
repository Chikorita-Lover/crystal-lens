using CrystalLens.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrystalLens.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<ASMFileViewModel> OpenFiles { get; } = [];
        public ICommand CloseTabCommand { get; }

        public MainViewModel()
        {
            CloseTabCommand = new RelayCommand(CloseTab_Execute);
        }

        private void CloseTab_Execute(object parameter)
        {
            OpenFiles.Remove((ASMFileViewModel)parameter);
        }
    }
}

using CrystalLens.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrystalLens.ViewModels
{
    public class MainViewModel : ObservableViewModel
    {
        public ObservableCollection<ASMFileViewModel> OpenFiles { get; } = [];
        public int SelectedFile
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public ICommand CloseTabCommand { get; }
        public ICommand NextTabCommand { get; }
        public ICommand PreviousTabCommand { get; }

        public MainViewModel()
        {
            CloseTabCommand = new RelayCommand(CloseTab_Execute);
            NextTabCommand = new RelayCommand(NextTab_Execute);
            PreviousTabCommand = new RelayCommand(PreviousTab_Execute);
        }

        private void CloseTab_Execute(object parameter)
        {
            OpenFiles.Remove((ASMFileViewModel)parameter);
        }

        private void NextTab_Execute(object parameter)
        {
            SelectedFile = (SelectedFile + 1) % OpenFiles.Count;
        }

        private void PreviousTab_Execute(object parameter)
        {
            SelectedFile = (SelectedFile - 1) % OpenFiles.Count;
        }
    }
}

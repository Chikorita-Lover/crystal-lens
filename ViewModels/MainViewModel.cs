using CrystalLens.Commands;
using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrystalLens.ViewModels
{
    public class MainViewModel : ObservableViewModel
    {
        public ASMProjectViewModel? OpenProject
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ASMFileViewModel> OpenFiles { get; } = [];
        public int SelectedFile
        {
            get;
            set { field = value; OnPropertyChanged(); }
        }
        public ICommand NextTabCommand { get; }
        public ICommand PreviousTabCommand { get; }

        public MainViewModel()
        {
            NextTabCommand = new RelayCommand(NextTab_Execute);
            PreviousTabCommand = new RelayCommand(PreviousTab_Execute);
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

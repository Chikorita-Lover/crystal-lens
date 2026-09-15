using CrystalLens.Commands;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace CrystalLens.ViewModels
{
    public class MainViewModel : ObservableViewModel
    {
        public ASMProjectViewModel? OpenProject
        {
            get; set { field = value; UpdateWindowTitle(); OnPropertyChanged(); }
        }
        public ObservableCollection<ASMFileViewModel> OpenFiles { get; } = [];
        public int SelectedFile
        {
            get;
            set { field = value; UpdateWindowTitle();  OnPropertyChanged(); }
        }
        public string WindowTitle
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public bool LoadingProject
        {
            get; set { field = value; OnPropertyChanged(); }
        }
        public ICommand NextTabCommand { get; }
        public ICommand PreviousTabCommand { get; }

        public MainViewModel()
        {
            NextTabCommand = new RelayCommand(NextTab_Execute);
            PreviousTabCommand = new RelayCommand(PreviousTab_Execute);

            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            StringBuilder sb = new();
            if (SelectedFile >= 0 && SelectedFile < OpenFiles.Count)
            {
                sb.Append($"{OpenFiles[SelectedFile].Name} - ");
            }
            if (OpenProject != null)
            {
                sb.Append($"{OpenProject.Model.Name} - ");
            }
            sb.Append("Crystal Lens");
            WindowTitle = sb.ToString();
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

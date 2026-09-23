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
        public ObservableCollection<DataTabViewModel> OpenTabs { get; } = [];
        public int SelectedTabIndex
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

        internal void OpenTab(DataTabViewModel tab)
        {
            OpenTabs.Add(tab);
            SelectedTabIndex = OpenTabs.Count - 1;
        }

        private void UpdateWindowTitle()
        {
            StringBuilder sb = new();
            if (SelectedTabIndex >= 0 && SelectedTabIndex < OpenTabs.Count)
            {
                sb.Append($"{OpenTabs[SelectedTabIndex].Name} - ");
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
            SelectedTabIndex = (SelectedTabIndex + 1) % OpenTabs.Count;
        }

        private void PreviousTab_Execute(object parameter)
        {
            SelectedTabIndex = (SelectedTabIndex - 1) % OpenTabs.Count;
        }
    }
}

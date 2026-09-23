using CrystalLens.Models;
using CrystalLens.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for PokemonSearchView.xaml
    /// </summary>
    public partial class PokemonSearchView : UserControl
    {
        private PokemonSearchViewModel ViewModel => (PokemonSearchViewModel)DataContext;

        public PokemonSearchView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!ViewModel.ShownFields.Contains(e.PropertyName))
            {
                e.Cancel = true;
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && sender is DataGridRow row && Window.GetWindow(row) is MainWindow window)
            {
                if (row.DataContext is PokemonStatsViewModel stats)
                {
                    ASMFile file = ((IASMData)stats.Model).File;
                    window.ViewModel.OpenTab(new ASMFileViewModel(file));
                }
            }
        }
    }
}

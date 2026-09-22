using CrystalLens.ViewModels;
using System.Windows.Controls;

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
    }
}

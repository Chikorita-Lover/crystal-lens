using CrystalLens.ViewModels;
using System.Windows;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for PokemonSearchOptionsWindow.xaml
    /// </summary>
    public partial class PokemonSearchOptionsWindow : Window
    {
        internal PokemonSearchOptions ViewModel => (PokemonSearchOptions)DataContext;

        public PokemonSearchOptionsWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

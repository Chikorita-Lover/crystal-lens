using CrystalLens.ViewModels;
using System.Windows.Controls;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for EncounterSetView.xaml
    /// </summary>
    public partial class EncounterSetView : UserControl
    {
        private EncounterSetViewModel ViewModel => (EncounterSetViewModel)DataContext;

        public EncounterSetView()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.AddEncounter();
        }
    }
}

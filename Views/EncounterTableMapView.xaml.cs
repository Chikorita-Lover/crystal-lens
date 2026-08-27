using CrystalLens.Models;
using System.Windows.Controls;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for EncounterTableMapView.xaml
    /// </summary>
    public partial class EncounterTableMapView : UserControl
    {
        private readonly EncounterTableMap encounterTables;

        public EncounterTableMapView(EncounterTableMap encounterTables)
        {
            InitializeComponent();

            this.encounterTables = encounterTables;

            mapBox.ItemsSource = encounterTables.GetNames();
            mapBox.SelectedIndex = 0;
        }

        private void MapSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimedEncounterTable encounterTable = encounterTables.Get((string)mapBox.SelectedItem);
            tableView.SetEncounters(encounterTable);
        }
    }
}

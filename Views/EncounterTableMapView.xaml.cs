using CrystalLens.Models;
using System.Windows;
using System.Windows.Controls;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for EncounterTableMapView.xaml
    /// </summary>
    public partial class EncounterTableMapView : UserControl
    {
        public static readonly DependencyProperty EncounterTablesProperty =
            DependencyProperty.Register(
                nameof(EncounterTables),
                typeof(EncounterTableMap),
                typeof(EncounterTableMapView),
                new PropertyMetadata(null, OnEncounterTablesChanged)
            );

        public EncounterTableMap EncounterTables
        {
            get => (EncounterTableMap)GetValue(EncounterTablesProperty);
            set => SetValue(EncounterTablesProperty, value);
        }

        public EncounterTableMapView()
        {
            InitializeComponent();
        }

        private static void OnEncounterTablesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is EncounterTableMapView view && view.EncounterTables != null)
            {
                view.mapBox.ItemsSource = view.EncounterTables.GetNames();
                view.mapBox.SelectedIndex = 0;
            }
        }

        private void MapSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EncounterTables != null && mapBox.SelectedIndex > -1)
            {
                TimedEncounterTable encounterTable = EncounterTables.Get((string)mapBox.SelectedItem);
                tableView.SetEncounters(encounterTable);
            }
        }
    }
}

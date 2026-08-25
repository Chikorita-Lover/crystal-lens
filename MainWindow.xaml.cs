using CrystalLens.Models;
using System.Windows;

namespace CrystalLens
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            EncounterTable<DayTime> encounterTable = CreateSampleTable();
            EncounterSet encounters = encounterTable.Get(DayTime.Day);
            encounterTableDisplay.ItemsSource = encounters.Encounters;
        }

        private static EncounterTable<DayTime> CreateSampleTable()
        {
            Encounter pidgey = new(2, "PIDGEY");
            Encounter sentret = new(2, "SENTRET");
            Encounter rattata = new(2, "RATTATA");
            Encounter hoppip = new(3, "HOPPIP");
            EncounterSet encounters = new([pidgey, sentret, pidgey, sentret, rattata, hoppip, hoppip], [30, 30, 20, 10, 5, 4, 1], 2);
            Dictionary<DayTime, EncounterSet> encounterSets = [];
            foreach (DayTime time in Enum.GetValues<DayTime>())
            {
                encounterSets.Add(time, encounters);
            }
            return new(encounterSets);
        }
    }
}
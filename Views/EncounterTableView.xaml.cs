using CrystalLens.Models;
using System.Windows.Controls;

namespace CrystalLens.Views
{
    /// <summary>
    /// Interaction logic for EncounterTableView.xaml
    /// </summary>
    public partial class EncounterTableView : UserControl
    {
        private TimedEncounterTable EncounterTable;

        public EncounterTableView()
        {
            InitializeComponent();
        }

        internal void SetEncounters(TimedEncounterTable encounterTable)
        {
            EncounterTable = encounterTable;
            PopulateDisplay();
        }

        private void PopulateDisplay()
        {
            EncounterSet encounterSet = EncounterTable.Get(DayTime.Day);
            List<EncounterDisplay> displays = [];
            foreach (Encounter encounter in encounterSet.Encounters)
            {
                EncounterDisplay display = new(encounter);
                displays.Add(display);
            }
            data.ItemsSource = displays;
        }

        public class EncounterDisplay
        {
            public string Name { get; }
            public string Level { get; }
            public string ImagePath { get; }

            internal EncounterDisplay(Encounter encounter)
            {
                Name = encounter.Name;
                Level = encounter.MinLevel == encounter.MaxLevel
                    ? encounter.MinLevel.ToString()
                    : $"{encounter.MinLevel} – {encounter.MaxLevel}";
                string SpriteName = Name == "UNOWN" ? "unown_a" : Name.ToLower();
                ImagePath = $@"C:\Users\cjgar\Git\celebi\gfx\pokemon\{SpriteName}\front.png";
            }
        }
    }
}

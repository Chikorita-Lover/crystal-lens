using CrystalLens.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrystalLens.ViewModels
{
    public class EncounterTableMapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public EncounterTableMap EncounterTables { get; set; }
        public string SelectedMap
        {
            get;
            set
            {
                field = value;
                SelectedEncounters = new(EncounterTables.Get(value));
            }
        }
        public ICollection<string> MapNames
        {
            get => EncounterTables.GetNames();
        }
        public EncounterTableViewModel SelectedEncounters
        {
            get;
            private set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        internal EncounterTableMapViewModel(EncounterTableMap encounterTables)
        {
            EncounterTables = encounterTables;
            SelectedMap = encounterTables.GetNames().First();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

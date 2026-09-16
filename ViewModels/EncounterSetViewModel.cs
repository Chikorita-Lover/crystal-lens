using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class EncounterSetViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public EncounterSet EncounterSet
        {
            get; set { field = value; PopulateEncounters(); }
        }
        public ObservableCollection<EncounterViewModel> Encounters { get; } = [];

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterSetViewModel(EncounterSet encounterSet)
        {
            EncounterSet = encounterSet;
        }

        private void PopulateEncounters()
        {
            Encounters.Clear();
            if (EncounterSet != null)
            {
                for (int i = 0; i < EncounterSet.Encounters.Count; i++)
                {
                    EncounterViewModel encounter = new(EncounterSet.Get(i), EncounterSet.GetProbability(i));
                    UpdateEncounterSprite(encounter);
                    Encounters.Add(encounter);
                    tracker.TryAddChildOf(encounter);
                    encounter.PropertyChanged += Encounter_PropertyChanged;
                }
            }
        }

        private void UpdateEncounterSprite(EncounterViewModel encounter)
        {
            ASMProject? project = IASMData.GetProject(EncounterSet);
            if (project != null)
            {
                encounter.Sprite.Path = Path.Combine(project.Path, $"gfx/pokemon/{encounter.Name.ToLower()}/front.png");
            }
        }

        private void Encounter_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EncounterViewModel encounter = (EncounterViewModel)sender;
            int index = Encounters.IndexOf(encounter);
            EncounterSet.Encounters[index] = encounter.ToModel();
            tracker.MarkAsUnsaved();

            UpdateEncounterSprite(encounter);
        }
    }
}

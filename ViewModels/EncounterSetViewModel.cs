using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.ViewModels
{
    public class EncounterSetViewModel : IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        public EncounterSet Model
        {
            get; set { field = value; PopulateEncounters(); }
        }
        public bool IsDynamic => Model.IsDynamic;
        public ObservableCollection<EncounterViewModel> Encounters { get; } = [];

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterSetViewModel(EncounterSet encounterSet)
        {
            Model = encounterSet;
        }

        private void PopulateEncounters()
        {
            Encounters.Clear();
            for (int i = 0; i < Model.Encounters.Count; i++)
            {
                AddEncounterViewModel(new(Model.Get(i), Model.GetProbability(i)));
            }
        }

        internal void AddEncounter(Encounter encounter, byte probability)
        {
            if (IsDynamic)
            {
                Model.Encounters.Add(encounter);
                Model.Probabilities.Add(probability);
                AddEncounterViewModel(new(encounter, probability));
            }
        }

        internal void AddEncounter()
        {
            string species = IASMData.GetProject(Model).Constants[ASMConstantGroup.Species].Keys.First();
            AddEncounter(new(5, species), 0);
        }

        private void AddEncounterViewModel(EncounterViewModel encounter)
        {
            UpdateEncounterSprite(encounter);
            Encounters.Add(encounter);
            tracker.TryAddChildOf(encounter);
            encounter.PropertyChanged += Encounter_PropertyChanged;
        }

        private void UpdateEncounterSprite(EncounterViewModel encounter)
        {
            ASMProject project = IASMData.GetProject(EncounterSet);
            encounter.Sprite.Path = Path.Combine(project.Path, $"gfx/pokemon/{encounter.Name.ToLower()}/front.png");
        }

        private void Encounter_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EncounterViewModel encounter = (EncounterViewModel)sender;
            int index = Encounters.IndexOf(encounter);
            Model.Encounters[index] = encounter.ToModel();
            tracker.MarkAsUnsaved();

            UpdateEncounterSprite(encounter);
        }
    }
}

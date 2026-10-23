using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class EncounterSetViewModel : ObservableViewModel, IChangeTracking
    {
        private readonly ChangeTracker tracker = new();
        private readonly Dictionary<EncounterViewModel, List<int>> _encountersToModelIndices = [];
        public EncounterSet Model
        {
            get; set { field = value; PopulateEncounters(); }
        }
        public bool IsDynamic => Model.IsDynamic;
        public ObservableCollection<EncounterViewModel> Encounters { get; } = [];
        public bool IsCombiningEntries
        {
            get;
            set
            {
                field = value;
                IsShowingMinMaxLevels = IsDynamic || value;
                PopulateEncounters();
                OnPropertyChanged();
            }
        }
        public bool IsShowingMinMaxLevels
        {
            get; set { field = value; OnPropertyChanged(); }
        }

        ChangeTracker IChangeTracking.Tracker => tracker;

        internal EncounterSetViewModel(EncounterSet encounterSet)
        {
            Model = encounterSet;
            IsCombiningEntries = false;
        }

        internal void PopulateEncounters()
        {
            Encounters.Clear();
            _encountersToModelIndices.Clear();
            
            if (IsCombiningEntries)
            {
                Dictionary<string, EncounterViewModel> speciesToEncounters = [];
                for (int i = 0; i < Model.Encounters.Count; i++)
                {
                    Encounter encounter = Model.Get(i);
                    EncounterViewModel encounterVM;
                    if (speciesToEncounters.TryGetValue(encounter.Species, out encounterVM))
                    {
                        encounterVM.MinLevel = Math.Min(encounterVM.MinLevel, encounter.MinLevel);
                        encounterVM.MaxLevel = Math.Max(encounterVM.MaxLevel, encounter.MaxLevel);
                        encounterVM.Probability += Model.GetProbability(i);

                        _encountersToModelIndices[encounterVM].Add(i);
                    }
                    else
                    {
                        encounterVM = new(encounter, Model.GetProbability(i));
                        speciesToEncounters.Add(encounter.Species, encounterVM);

                        _encountersToModelIndices.Add(encounterVM, [i]);
                    }
                }
                foreach (EncounterViewModel encounter in speciesToEncounters.Values.OrderByDescending(encounter => encounter.Probability))
                {
                    AddEncounterViewModel(encounter);
                }
            }
            else
            {
                for (int i = 0; i < Model.Encounters.Count; i++)
                {
                    AddEncounterViewModel(new(Model.Get(i), Model.GetProbability(i)));
                }
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
            ASMProject project = IASMData.GetProject(Model);
            encounter.PicPath = project.GetPicPath(encounter.Species);
        }

        private void Encounter_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(EncounterViewModel.PicPath))
            {
                EncounterViewModel encounterVM = (EncounterViewModel)sender;
                if (IsCombiningEntries)
                {
                    foreach (int index in _encountersToModelIndices[encounterVM])
                    {
                        Encounter oldEncounter = Model.Encounters[index];
                        Model.Encounters[index] = new(oldEncounter.MinLevel, oldEncounter.MaxLevel, encounterVM.Species);
                    }
                }
                else
                {
                    int index = Encounters.IndexOf(encounterVM);
                    Model.Encounters[index] = encounterVM.ToModel();
                }

                if (e.PropertyName == nameof(EncounterViewModel.Species))
                {
                    UpdateEncounterSprite(encounterVM);
                }

                tracker.MarkAsUnsaved();
            }
        }
    }
}

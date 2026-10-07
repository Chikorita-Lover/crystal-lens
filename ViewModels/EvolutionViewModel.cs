using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CrystalLens.ViewModels
{
    public class EvolutionViewModel : ObservableViewModel
    {
        private static readonly Dictionary<string, string[]> TriggerLabels = GenerateTriggerLabels();
        public EvolutionMoveSet.Evolution Model;
        public string Method
        {
            get => Model.Method;
            set { Model.Method = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Trigger> Triggers { get; }
        public string Species
        {
            get => Model.Species;
            set { Model.Species = value; OnPropertyChanged(); }
        }

        public EvolutionViewModel(EvolutionMoveSet.Evolution model)
        {
            Model = model;
            Triggers = new(model.Triggers.Select((value, index) =>
            {
                string label = TriggerLabels.TryGetValue(model.Method, out string[]? labels) && index < labels.Length
                    ? labels[index] : $"Trigger {index + 1}";
                Trigger trigger = new(label, value);
                trigger.PropertyChanged += Trigger_PropertyChanged;
                return trigger;
            }));

            Triggers.CollectionChanged += Triggers_CollectionChanged;
        }

        public EvolutionViewModel() : this(new("EVOLVE_LEVEL", ["1"], "BULBASAUR"))
        { }

        private void Trigger_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Trigger trigger = (Trigger)sender;
            int index = Triggers.IndexOf(trigger);
            Model.Triggers[index] = trigger.Value;
        }

        private void Triggers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Model.Triggers = [.. Triggers.Select(o => o.Value)];
        }

        private static Dictionary<string, string[]> GenerateTriggerLabels()
        {
            Dictionary<string, string[]> labels = [];
            labels.Add("EVOLVE_LEVEL", ["Level"]);
            labels.Add("EVOLVE_ITEM", ["Item"]);
            labels.Add("EVOLVE_TRADE", ["Hold item"]);
            labels.Add("EVOLVE_HAPPINESS", ["Time"]);
            labels.Add("EVOLVE_STAT", ["Level", "Stat comparison"]);
            return labels;
        }

        public class Trigger : ObservableViewModel
        {
            public string Label { get; }
            public string Value
            {
                get; set { field = value; OnPropertyChanged(); }
            }

            public Trigger(string label, string value)
            {
                Label = label;
                Value = value;
            }
        }
    }
}

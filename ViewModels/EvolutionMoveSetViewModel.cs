using CrystalLens.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CrystalLens.ViewModels
{
    public partial class EvolutionMoveSetViewModel
    {
        public EvolutionMoveSet Model { get; }
        public ObservableCollection<EvolutionViewModel> Evolutions { get; }
        public ObservableCollection<MoveEntry> Moves { get; }

        public EvolutionMoveSetViewModel(EvolutionMoveSet model)
        {
            Model = model;
            Evolutions = [.. model.Evolutions.Select(m => new EvolutionViewModel(m))];
            Moves = [.. model.Moves.Select(m => new MoveEntry(m))];

            Evolutions.CollectionChanged += Evolutions_CollectionChanged;
            Moves.CollectionChanged += Moves_CollectionChanged;
        }

        private void Evolutions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (EvolutionViewModel evolution in e.NewItems)
                {
                    Model.Evolutions.Add(evolution.Model);
                }
            }
        }

        private void Moves_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (MoveEntry move in e.NewItems)
                {
                    Model.Moves.Add(move.Model);
                }
            }
        }

        public class MoveEntry : ObservableViewModel
        {
            public EvolutionMoveSet.MoveEntry Model { get; }
            public byte Level
            {
                get => Model.Level;
                set { Model.Level = value; OnPropertyChanged(); }
            }
            public string Move
            {
                get => Model.Move;
                set { Model.Move = value; OnPropertyChanged(); }
            }

            public MoveEntry(EvolutionMoveSet.MoveEntry model)
            {
                Model = model;
            }

            public MoveEntry() : this(new(1, "NO_MOVE"))
            { }
        }
    }
}

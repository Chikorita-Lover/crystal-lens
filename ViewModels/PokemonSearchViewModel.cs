using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class PokemonSearchViewModel : DataTabViewModel
    {
        internal readonly List<string> ShownFields = [];
        private readonly ASMProject _project;

        public override string Name => "Pokémon Search";
        internal PokemonSearchOptions Options { get; }
        public ObservableCollection<PokemonStatsViewModel> Entries { get; } = [];

        internal PokemonSearchViewModel(ASMProject project, PokemonSearchOptions options)
        {
            _project = project;
            Options = options;
            ShownFields.AddRange(options.Fields.SelectMany(field => field.IsShown ? field.Type.PropertyNames : []));

            PopulateEntries(options);
        }

        private void PopulateEntries(PokemonSearchOptions options)
        {
            foreach (ASMFile file in _project.ProjectFiles)
            {
                if (file.Get(file.Labels.First()) is PokemonStats stats)
                {
                    PokemonStatsViewModel entry = new(stats);
                    if (!options.IsFiltered || options.FilterKey.ValueFunction.Invoke(entry).Any(o => o.ToString() == options.FilterValue))
                    {
                        entry.FrontSprite.PlayAnimationOnLoad = false;
                        Entries.Add(entry);
                    }
                }
            }
        }
    }
}

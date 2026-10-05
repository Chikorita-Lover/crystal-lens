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
            foreach (ASMFile.Header header in _project.FileHeaders)
            {
                if (ASMDataType.PokemonStats.PathPredicate.Invoke(header.RelativePath) && ASMFile.TryReadFile(_project, header, out ASMFile file))
                {
                    PokemonStatsViewModel entry = new((PokemonStats)file.Get(string.Empty));
                    if (!options.IsFiltered || options.FilterKey.Selector.Invoke(entry).Any(o => o.ToString() == options.FilterValue))
                    {
                        Entries.Add(entry);
                    }
                }
            }
        }
    }
}

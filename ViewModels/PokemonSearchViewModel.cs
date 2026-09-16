using CrystalLens.Models;
using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class PokemonSearchViewModel : DataTabViewModel
    {
        private readonly ASMProject _project;

        public override string Name => "Pokémon Search";
        public ObservableCollection<PokemonStatsViewModel> Entries { get; } = [];

        public PokemonSearchViewModel(ASMProject project)
        {
            _project = project;

            PopulateEntries();
        }

        private void PopulateEntries()
        {
            foreach (ASMFile file in _project.ProjectFiles)
            {
                if (file.Get(file.Labels.First()) is PokemonStats stats)
                {
                    PokemonStatsViewModel entry = new(stats);
                    entry.FrontSprite.PlayAnimationOnLoad = false;
                    Entries.Add(entry);
                }
            }
        }
    }
}

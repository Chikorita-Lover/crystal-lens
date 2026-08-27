using System.Collections.ObjectModel;

namespace CrystalLens.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<ASMFileViewModel> OpenFiles { get; } = [];
    }
}

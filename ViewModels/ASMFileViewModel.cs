using CrystalLens.Models;

namespace CrystalLens.ViewModels
{
    public class ASMFileViewModel
    {
        public readonly ASMFile File;
        public string Name
        {
            get => File.Name;
        }
        public string Path
        {
            get => File.Path;
        }
        public object Data { get; }

        public ASMFileViewModel(ASMFile file)
        {
            File = file;
            Data = CreateDataViewModel(file.Get(file.Labels.First()));
        }

        private static object CreateDataViewModel(IASMData data)
        {
            if (data is EncounterTableMap encounterTableMap)
            {
                return new EncounterTableMapViewModel(encounterTableMap);
            }
            if (data is EncounterSetMap encounterSetMap)
            {
                return new EncounterSetMapViewModel(encounterSetMap);
            }
            if (data is TimedEncounterTable encounterTable)
            {
                return new EncounterTableViewModel(encounterTable);
            }
            if (data is EncounterSet encounterSet)
            {
                return new EncounterSetViewModel(encounterSet);
            }
            return null;
        }
    }
}

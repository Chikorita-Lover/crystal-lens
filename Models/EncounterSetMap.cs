namespace CrystalLens.Models
{
    public class EncounterSetMap : IASMData
    {
        private readonly ASMFile _file;
        public Dictionary<string, EncounterSet> EncounterSets;

        ASMFile IASMData.File => _file;

        public EncounterSetMap(ASMFile file, Dictionary<string, EncounterSet> encounterSets)
        {
            _file = file;
            EncounterSets = encounterSets;
        }

        public EncounterSet Get(string name)
        {
            return EncounterSets[name];
        }

        public ICollection<string> GetNames()
        {
            return EncounterSets.Keys;
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EncounterSetMap;
        }

        public class Serializer : ASMSerializer
        {
            private const string DefCommand = "def_water_wildmons";
            private const string EndCommand = "end_water_wildmons";

            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                string value;
                Dictionary<string, EncounterSet> encounterSets = [];
                while ((value = reader.Read()) != "-1")
                {
                    EncounterSet encounterSet = (EncounterSet)ASMSerializers.EncounterSet.ReadAssembly(reader, file);
                    encounterSets.Add(value, encounterSet);
                }

                return new EncounterSetMap(file, encounterSets);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                EncounterSetMap encounterSets = (EncounterSetMap)data;
                writer.NewLine();
                foreach (string name in encounterSets.GetNames())
                {
                    writer.WriteCommand(new(DefCommand, [name]));
                    ASMSerializers.EncounterSet.WriteAssembly(writer, encounterSets.Get(name));
                    writer.WriteCommand(new(EndCommand));
                    writer.NewLine();
                }
                writer.DeclareBytes([-1], "end");
            }
        }
    }
}

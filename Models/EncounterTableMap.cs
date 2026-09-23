namespace CrystalLens.Models
{
    /// <summary>
    /// Specialized class for dictionary of location name to EncounterTable
    /// </summary>
    public class EncounterTableMap : IASMData
    {
        private readonly ASMFile _file;
        public Dictionary<string, EncounterTable> EncounterTables;

        ASMFile IASMData.File => _file;

        public EncounterTableMap(ASMFile file, Dictionary<string, EncounterTable> encounterTables)
        {
            _file = file;
            EncounterTables = encounterTables;
        }

        public EncounterTable Get(string name)
        {
            return EncounterTables[name];
        }

        public ICollection<string> GetNames()
        {
            return EncounterTables.Keys;
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EncounterTableMap;
        }

        public class Serializer : ASMSerializer
        {
            private const string DefCommand = "def_grass_wildmons";
            private const string EndCommand = "end_grass_wildmons";

            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                string value;
                Dictionary<string, EncounterTable> encounterTables = [];
                while ((value = reader.Read()) != "-1")
                {
                    EncounterTable encounterTable = (EncounterTable)ASMSerializers.EncounterTable.ReadAssembly(reader, file);
                    encounterTables.Add(value, encounterTable);
                }

                return new EncounterTableMap(file, encounterTables);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                EncounterTableMap encounterTables = (EncounterTableMap)data;

                writer.NewLine();
                foreach (string name in encounterTables.GetNames())
                {
                    writer.WriteCommand(new(DefCommand, [name]));
                    ASMSerializers.EncounterTable.WriteAssembly(writer, encounterTables.Get(name));
                    writer.WriteCommand(new(EndCommand));
                    writer.NewLine();
                }
                writer.DeclareBytes([-1], "end");
            }
        }
    }
}

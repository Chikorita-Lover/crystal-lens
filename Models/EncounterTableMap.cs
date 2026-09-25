namespace CrystalLens.Models
{
    /// <summary>
    /// Specialized class for dictionary of location name to EncounterTable
    /// </summary>
    public class EncounterTableMap : IASMData
    {
        private readonly ASMFile _file;
        public Dictionary<string, EncounterTable> EncounterTables;
        public Dictionary<string, byte[]> EncounterRates;

        ASMFile IASMData.File => _file;

        public EncounterTableMap(ASMFile file, Dictionary<string, EncounterTable> encounterTables, Dictionary<string, byte[]> encounterRates)
        {
            _file = file;
            EncounterTables = encounterTables;
            EncounterRates = encounterRates;
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
                string mapName;
                Dictionary<string, EncounterTable> encounterTables = [];
                Dictionary<string, byte[]> encounterRates = [];
                while ((mapName = reader.Read()) != "-1")
                {
                    byte[] mapEncounterRates = new byte[3];
                    for (int i = 0; i < mapEncounterRates.Length; i++)
                    {
                        string parameter = reader.Read();
                        mapEncounterRates[i] = byte.Parse(parameter.Split(" percent")[0]);
                    }
                    encounterRates.Add(mapName, mapEncounterRates);

                    EncounterTable encounterTable = (EncounterTable)ASMSerializers.EncounterTable.ReadAssembly(reader, file);
                    encounterTables.Add(mapName, encounterTable);
                }

                return new EncounterTableMap(file, encounterTables, encounterRates);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                EncounterTableMap encounterTables = (EncounterTableMap)data;

                writer.NewLine();
                foreach (string name in encounterTables.GetNames())
                {
                    writer.WriteCommand(new(DefCommand, [name]));

                    string[] rates = [.. encounterTables.EncounterRates[name].Select(rate => $"{rate} percent")];
                    writer.DeclareBytes(rates, "encounter rates: morn/day/nite");
                    ASMSerializers.EncounterTable.WriteAssembly(writer, encounterTables.Get(name));

                    writer.WriteCommand(new(EndCommand));
                    writer.NewLine();
                }
                writer.DeclareBytes([-1], "end");
            }
        }
    }
}

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
            internal override IASMData ReadAssembly(Queue<ASMCommand> commands, ASMFile file)
            {
                Dictionary<string, EncounterTable> encounterTables = [];
                ASMCommand command;
                while ((command = commands.Dequeue()) != null && command.Command != "db")
                {
                    command.VerifyOrThrow("def_grass_wildmons");

                    EncounterTable encounterTable = (EncounterTable)ASMSerializers.EncounterTable.ReadAssembly(commands, file);
                    encounterTables.Add(command.Get(0), encounterTable);

                    command = commands.Dequeue();
                    command.VerifyOrThrow("end_grass_wildmons");
                }

                return new EncounterTableMap(file, encounterTables);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                EncounterTableMap encounterTables = (EncounterTableMap)data;

                commands.Enqueue(new("", []));
                foreach (string name in encounterTables.GetNames())
                {
                    commands.Enqueue(new("def_grass_wildmons", [name]));
                    ASMSerializers.EncounterTable.WriteAssembly(commands, encounterTables.Get(name));
                    commands.Enqueue(new("end_grass_wildmons", []));
                    commands.Enqueue(new());
                }
                commands.Enqueue(new("db", ["-1"], "end"));
            }
        }
    }
}

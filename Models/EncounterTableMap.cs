namespace CrystalLens.Models
{
    /// <summary>
    /// Specialized class for dictionary of location name to EncounterTable
    /// </summary>
    public class EncounterTableMap
    {
        public Dictionary<string, TimedEncounterTable> EncounterTables;

        public EncounterTableMap(Dictionary<string, TimedEncounterTable> encounterTables)
        {
            EncounterTables = encounterTables;
        }

        public TimedEncounterTable Get(string name)
        {
            return EncounterTables[name];
        }

        public ICollection<string> GetNames()
        {
            return EncounterTables.Keys;
        }

        public class Serializer : ASMSerializer<EncounterTableMap>
        {
            internal override EncounterTableMap ReadAssembly(Queue<ASMCommand> commands)
            {
                Dictionary<string, TimedEncounterTable> encounterTables = [];
                ASMCommand command;
                while ((command = commands.Dequeue()) != null && command.Command != "db")
                {
                    command.VerifyOrThrow("def_grass_wildmons");

                    TimedEncounterTable encounterTable = ASMSerializers.EncounterTable.ReadAssembly(commands);
                    encounterTables.Add(command.Get(0), encounterTable);

                    command = commands.Dequeue();
                    command.VerifyOrThrow("end_grass_wildmons");
                }

                return new EncounterTableMap(encounterTables);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, EncounterTableMap data)
            {
                commands.Enqueue(new("", []));
                foreach (string name in data.GetNames())
                {
                    commands.Enqueue(new("def_grass_wildmons", [name]));
                    ASMSerializers.EncounterTable.WriteAssembly(commands, data.Get(name));
                    commands.Enqueue(new("end_grass_wildmons", []));
                    commands.Enqueue(new());
                }
                commands.Enqueue(new("db", ["-1"], "end"));
            }
        }
    }
}

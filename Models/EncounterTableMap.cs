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

        internal static EncounterTableMap ReadASM(Queue<ASMCommand> commands)
        {
            Dictionary<string, TimedEncounterTable> encounterTables = [];
            ASMCommand command;
            while ((command = commands.Dequeue()) != null && command.Command != "db")
            {
                command.VerifyOrThrow("def_grass_wildmons");

                TimedEncounterTable encounterTable = TimedEncounterTable.ReadAssembly(commands);
                encounterTables.Add(command.Get(0), encounterTable);

                command = commands.Dequeue();
                command.VerifyOrThrow("end_grass_wildmons");
            }

            return new EncounterTableMap(encounterTables);
        }
    }
}

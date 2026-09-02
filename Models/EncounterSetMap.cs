namespace CrystalLens.Models
{
    public class EncounterSetMap : IASMData
    {
        public Dictionary<string, EncounterSet> EncounterSets;

        public EncounterSetMap(Dictionary<string, EncounterSet> encounterSets)
        {
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
            private static readonly string defCommand = "def_water_wildmons";
            private static readonly string endCommand = "end_water_wildmons";

            internal override IASMData ReadAssembly(Queue<ASMCommand> commands)
            {
                Dictionary<string, EncounterSet> encounterSets = [];
                ASMCommand command;
                while ((command = commands.Dequeue()) != null && command.Command != "db")
                {
                    command.VerifyOrThrow(defCommand);

                    EncounterSet encounterSet = (EncounterSet)ASMSerializers.EncounterSet.ReadAssembly(commands);
                    encounterSets.Add(command.Get(0), encounterSet);

                    command = commands.Dequeue();
                    command.VerifyOrThrow(endCommand);
                }

                return new EncounterSetMap(encounterSets);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                EncounterSetMap encounterSets = (EncounterSetMap)data;
                commands.Enqueue(new());
                foreach (string name in encounterSets.GetNames())
                {
                    commands.Enqueue(new(defCommand, [name]));
                    ASMSerializers.EncounterSet.WriteAssembly(commands, encounterSets.Get(name));
                    commands.Enqueue(new(endCommand, []));
                    commands.Enqueue(new());
                }
                commands.Enqueue(new("db", ["-1"], "end"));
            }
        }
    }
}

namespace CrystalLens.Models
{
    public class EncounterSet : IASMData
    {
        private readonly ASMFile _file;
        public List<Encounter> Encounters;
        public List<int> Probabilities;
        public int EncounterRate;

        ASMFile IASMData.File => _file;

        public EncounterSet(ASMFile file, List<Encounter> encounters, List<int> probabilities, int encounterRate)
        {
            _file = file;
            Encounters = encounters;
            Probabilities = probabilities;
            EncounterRate = encounterRate;
        }

        public Encounter Get(int index)
        {
            return Encounters[index];
        }

        public int GetProbability(int index)
        {
            return Probabilities[index];
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EncounterSet;
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(Queue<ASMCommand> commands, ASMFile file)
            {
                ASMCommand command = commands.Dequeue();
                command.VerifyOrThrow("db");

                int count = command.Parameters.Length;
                int encounterRate;

                string parameter = command.Get(0);
                try
                {
                    encounterRate = int.Parse(parameter.Split(" percent")[0]);
                }
                catch (FormatException e)
                {
                    throw new InvalidOperationException("Invalid encounter rate format: " + command + ": " + e);
                }

                List<int> probabilities = [60, 30, 10];
                List<Encounter> encounters = [];
                for (int j = 0; j < probabilities.Count; j++)
                {
                    Encounter encounter = Encounter.ReadAssembly(commands);
                    encounters.Add(encounter);
                }

                return new EncounterSet(file, encounters, probabilities, encounterRate);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                EncounterSet encounterSet = (EncounterSet)data;

                string rate = PercentFromInt(encounterSet.EncounterRate);
                commands.Enqueue(new("db", [rate], "encounter rate"));

                foreach (Encounter encounter in encounterSet.Encounters)
                {
                    commands.Enqueue(new("db", [encounter.MinLevel.ToString(), encounter.Name]));
                }
            }

            private static string PercentFromInt(int value)
            {
                return $"{value} percent";
            }
        }
    }
}

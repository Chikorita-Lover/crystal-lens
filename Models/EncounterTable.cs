namespace CrystalLens.Models
{
    public record EncounterTable(Dictionary<DayTime, EncounterSet> EncounterSets) : IASMData
    {
        public EncounterSet Get(DayTime time)
        {
            return EncounterSets[time];
        }

        public void Add(DayTime time, EncounterSet encounters)
        {
            EncounterSets[time] = encounters;
        }

        public ASMSerializer GetSerializer()
        {
            return ASMSerializers.EncounterTable;
        }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(Queue<ASMCommand> commands)
            {
                ASMCommand command = commands.Dequeue();
                command.VerifyOrThrow("db");

                int count = command.Parameters.Length;
                int[] encounterRates = new int[count];
                for (int i = 0; i < count; i++)
                {
                    string parameter = command.Get(i);
                    try
                    {
                        encounterRates[i] = int.Parse(parameter.Split(" percent")[0]);
                    }
                    catch (FormatException e)
                    {
                        throw new InvalidOperationException("Invalid encounter rate format: " + command + ": " + e);
                    }
                }

                List<int> probabilities = [30, 30, 20, 10, 5, 4, 1];
                Dictionary<DayTime, EncounterSet> encounterSets = [];
                for (int i = 0; i < count; i++)
                {
                    List<Encounter> encounters = [];
                    for (int j = 0; j < probabilities.Count; j++)
                    {
                        Encounter encounter = Encounter.ReadAssembly(commands);
                        encounters.Add(encounter);
                    }
                    DayTime time = Enum.GetValues<DayTime>()[i];
                    encounterSets[time] = new(encounters, probabilities, encounterRates[i]);
                }

                return new EncounterTable(encounterSets);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                EncounterTable encounterTable = (EncounterTable)data;

                List<string> percents = [];
                foreach (DayTime time in Enum.GetValues<DayTime>())
                {
                    int rate = encounterTable.EncounterSets[time].EncounterRate;
                    percents.Add(PercentFromInt(rate));
                }
                commands.Enqueue(new("db", percents.ToArray(), "encounter rates: morn/day/nite"));

                foreach (DayTime time in Enum.GetValues<DayTime>())
                {
                    commands.Enqueue(new("", [], time.ToString().ToLower()));
                    EncounterSet encounters = encounterTable.EncounterSets[time];
                    foreach (Encounter encounter in encounters.Encounters)
                    {
                        commands.Enqueue(new("db", [encounter.MinLevel.ToString(), encounter.Name]));
                    }
                }
            }

            private static string PercentFromInt(int value)
            {
                return $"{value} percent";
            }
        }
    }
}

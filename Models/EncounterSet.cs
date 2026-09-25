namespace CrystalLens.Models
{
    public class EncounterSet : IASMData
    {
        private readonly ASMFile _file;
        public readonly bool IsDynamic;
        public List<Encounter> Encounters;
        public List<byte> Probabilities;

        ASMFile IASMData.File => _file;

        public EncounterSet(ASMFile file, List<Encounter> encounters, List<byte> probabilities, bool isDynamic = false)
        {
            _file = file;
            Encounters = encounters;
            Probabilities = probabilities;
            IsDynamic = isDynamic;
        }

        public Encounter Get(int index)
        {
            return Encounters[index];
        }

        public byte GetProbability(int index)
        {
            return Probabilities[index];
        }

        public ASMSerializer GetSerializer()
        {
            return IsDynamic ? ASMSerializers.DynamicEncounterSet : ASMSerializers.EncounterSet;
        }

        public class Serializer : ASMSerializer
        {
            private readonly bool _isDynamic;

            public Serializer(bool isDynamic)
            {
                _isDynamic = isDynamic;
            }

            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                List<byte> probabilities;
                List<Encounter> encounters = [];
                
                if (_isDynamic)
                {
                    probabilities = [];
                    byte probability;
                    while ((probability = reader.ReadByte()) != 255)
                    {
                        probabilities.Add(probability);
                        string name = reader.Read();
                        encounters.Add(new(reader.ReadByte(), reader.ReadByte(), name));
                    }
                }
                else
                {
                    probabilities = [60, 30, 10];
                    for (byte b = 0; b < probabilities.Count; b++)
                    {
                        byte level = reader.ReadByte();
                        string name = reader.Read();
                        encounters.Add(new(level, name));
                    }
                }

                return new EncounterSet(file, encounters, probabilities, _isDynamic);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                EncounterSet encounterSet = (EncounterSet)data;

                if (_isDynamic)
                {
                    writer.Comment("%, species, min, max");
                }

                for (int i = 0; i < encounterSet.Encounters.Count; i++)
                {
                    Encounter encounter = encounterSet.Get(i);
                    if (_isDynamic)
                    {
                        writer.DeclareBytes([encounterSet.GetProbability(i), encounter.Species, encounter.MinLevel, encounter.MaxLevel]);
                    }
                    else
                    {
                        writer.DeclareBytes([encounter.MinLevel, encounter.Species]);
                    }
                }
                if (_isDynamic)
                {
                    writer.DeclareBytes([-1]);
                }
            }
        }
    }
}

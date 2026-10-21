namespace CrystalLens.Models
{
    public class AnimationPointerTable : IASMData
    {
        private readonly List<string> _animations;
        public string this[int index] { get => _animations[index]; }

        public ASMFile File { get; }
        public ASMDataType Type => ASMDataType.AnimPointers;

        public AnimationPointerTable(ASMFile file, List<string> animations)
        {
            File = file;
            _animations = animations;
        }

        public class Serializer : ASMSerializer
        {
            private readonly int _length;

            public Serializer(int length)
            {
                _length = length;
            }

            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                List<string> animations = [];
                for (int i = 0; i < _length; i++)
                {
                    animations.Add(reader.Read());
                }
                return new AnimationPointerTable(file, animations);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                AnimationPointerTable animPointers = (AnimationPointerTable)data;
                writer.WriteCommand(new("table_width", [2]));
                foreach (string animation in animPointers._animations)
                {
                    writer.DeclareWords([animation]);
                }
                writer.WriteCommand(new("assert_table_length", ["NUM_POKEMON"]));
            }
        }
    }
}

namespace CrystalLens.Models
{
    public class SpriteAnimation : IASMData
    {
        public const byte EndAnimCommand = 0xff;
        public const byte SetRepeatCommand = EndAnimCommand - 1;
        public const byte DoRepeatCommand = SetRepeatCommand - 1;

        private readonly List<Command> _commands;
        private readonly ASMFile _file;

        public int CommandCount => _commands.Count;

        ASMFile IASMData.File => _file;

        public SpriteAnimation(ASMFile file, List<Command> commands)
        {
            _file = file;
            _commands = commands;
        }

        public Command Get(int index)
        {
            return _commands[index];
        }

        public ASMSerializer GetSerializer()
        {
            throw new NotImplementedException();
        }

        public abstract record Command()
        { }

        public record Frame(byte Index, byte Duration) : Command
        { }

        public record SetRepeat(byte Count) : Command
        { }

        public record DoRepeat(byte Index) : Command
        { }

        public class Serializer : ASMSerializer
        {
            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                List<Command> animCommands = [];
                byte value;
                while ((value = reader.ReadByte()) != 0xff)
                {
                    Command animCommand = value switch
                    {
                        SetRepeatCommand => new SetRepeat(reader.ReadByte()),
                        DoRepeatCommand => new DoRepeat(reader.ReadByte()),
                        _ => new Frame(value, reader.ReadByte())
                    };
                    animCommands.Add(animCommand);
                }
                return new SpriteAnimation(file, animCommands);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                throw new NotImplementedException();
            }
        }
    }
}

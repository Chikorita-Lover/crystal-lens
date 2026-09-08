namespace CrystalLens.Models
{
    public class SpriteAnimation : IASMData
    {
        private readonly List<Command> commands;

        public int CommandCount => commands.Count;

        public SpriteAnimation(List<Command> commands)
        {
            this.commands = commands;
        }

        public Command Get(int index)
        {
            return commands[index];
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
            private static readonly string endanimCommand = "endanim";

            internal override IASMData ReadAssembly(Queue<ASMCommand> commands)
            {
                List<Command> animCommands = [];
                ASMCommand command;
                while ((command = commands.Dequeue()).Command != endanimCommand)
                {
                    Command animCommand = command.Command switch
                    {
                        "frame" => new Frame(command.GetByte(0), command.GetByte(1)),
                        "setrepeat" => new SetRepeat(command.GetByte(0)),
                        "dorepeat" => new DoRepeat(command.GetByte(0)),
                        _ => throw new InvalidOperationException()
                    };
                    animCommands.Add(animCommand);
                }
                return new SpriteAnimation(animCommands);
            }

            internal override void WriteAssembly(Queue<ASMCommand> commands, IASMData data)
            {
                throw new NotImplementedException();
            }
        }
    }
}

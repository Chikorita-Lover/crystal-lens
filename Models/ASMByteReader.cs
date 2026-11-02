using System.IO;
using System.Text.RegularExpressions;

namespace CrystalLens.Models
{
    internal class ASMByteReader : ASMReader
    {
        private static readonly string[] IgnoredMacros = ["table_width", "assert_table_length"];
        private readonly List<object> _stream = [];
        private readonly Dictionary<string, int> _labelsToPositions = [];
        private int _position;

        public ASMByteReader(ASMProject project, StreamReader reader) : base(project, reader)
        { }

        public string ReadSymbol()
        {
            if (_position >= _stream.Count)
            {
                string? line = ReadLine();
                if (line == null)
                {
                    throw new EndOfStreamException();
                }

                if (ASMCommand.HasLabel(line))
                {
                    string label = Regex.Match(line, "[A-Za-z_][\\w#$@]+(\\.[A-Za-z_][\\w#$@]+)?").Value;
                    _labelsToPositions.Add(label, _position);
                }
                else
                {
                    ASMCommand command = ASMCommand.FromLine(line);
                    ProcessCommand(command);
                }

                return ReadSymbol();
            }
            return _stream[_position++].ToString() ?? throw new EndOfStreamException();
        }

        public byte ReadByte()
        {
            string? value = ReadSymbol();
            return (byte)(int.Parse(value ?? "0") % 256);
        }

        public short ReadWord()
        {
            string? value = ReadSymbol();
            return (short)(int.Parse(value ?? "0") % (1 << 16));
        }

        public void JumpTo(string label)
        {
            bool hasLabel;
            while (!(hasLabel = _labelsToPositions.ContainsKey(label)) && ReadSymbol() != null)
            { }
            if (!hasLabel)
            {
                throw new KeyNotFoundException("The specified label is not defined in the provided ASM file.");
            }
            _position = _labelsToPositions[label];
        }

        protected override void ProcessCustomCommand(ASMCommand command)
        {
            if (IgnoredMacros.Contains(command.Command))
            {
                return;
            }
            switch (command.Command)
            {
                case "frame":
                    _stream.Add(command.GetByte(0));
                    _stream.Add(command.GetByte(1));
                    break;
                case "endanim":
                    _stream.Add(SpriteAnimation.EndAnimCommand);
                    break;
                case "setrepeat":
                    _stream.Add(SpriteAnimation.SetRepeatCommand);
                    _stream.Add(command.GetByte(0));
                    break;
                case "dorepeat":
                    _stream.Add(SpriteAnimation.DoRepeatCommand);
                    _stream.Add(command.GetByte(0));
                    break;
                case "dba_pics":
                    _stream.Add(command.Count >= 1 ? command.Get(0) : -1);
                    _stream.Add(command.Count >= 2 ? command.Get(1) : -1);
                    break;
                case "RGB":
                    for (int i = 0; i < command.Count / 3; i++)
                    {
                        short r = (short)((1 << 0) * command.GetByte(i * 3));
                        short g = (short)((1 << 5) * command.GetByte(i * 3 + 1));
                        short b = (short)((1 << 10) * command.GetByte(i * 3 + 2));
                        _stream.Add(r + g + b);
                    }
                    break;
                default:
                    for (int i = 0; i < command.Count; i++)
                    {
                        _stream.Add(command.Get(i));
                    }
                    break;
            }
        }
    }
}

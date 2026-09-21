using System.IO;

namespace CrystalLens.Models
{
    public class ASMReader
    {
        private readonly StreamReader _reader;
        private readonly Queue<object> _queue = [];
        private int _line = 1;

        public ASMReader(string path)
        {
            _reader = new(path);
        }

        public void Close()
        {
            _reader.Close();
        }

        public void AdvanceToLine(int target)
        {
            while (_line < target)
            {
                _reader.ReadLine();
                _line++;
            }
        }

        public string? Read()
        {
            if (_queue.Count == 0)
            {
                string? line = _reader.ReadLine();
                _line++;
                if (line == null)
                {
                    return null;
                }
                ASMCommand command = ASMCommand.FromLine(line);
                RunCommand(command);
                return Read();
            }
            return _queue.Dequeue().ToString();
        }

        public byte ReadByte()
        {
            string? value = Read();
            return byte.Parse(value ?? "0");
        }

        private void RunCommand(ASMCommand command)
        {
            switch (command.Command)
            {
                case "frame":
                    _queue.Enqueue(command.GetByte(0));
                    _queue.Enqueue(command.GetByte(1));
                    break;
                case "endanim":
                    _queue.Enqueue(SpriteAnimation.EndAnimCommand);
                    break;
                case "setrepeat":
                    _queue.Enqueue(SpriteAnimation.SetRepeatCommand);
                    _queue.Enqueue(command.GetByte(0));
                    break;
                case "dorepeat":
                    _queue.Enqueue(SpriteAnimation.DoRepeatCommand);
                    _queue.Enqueue(command.GetByte(0));
                    break;
                default:
                    for (int i = 0; i < command.Count; i++)
                    {
                        _queue.Enqueue(command.Get(i));
                    }
                    break;
            }
        }
    }
}

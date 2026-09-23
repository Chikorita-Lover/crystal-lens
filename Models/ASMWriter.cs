using System.IO;

namespace CrystalLens.Models
{
    public class ASMWriter
    {
        private readonly StreamWriter _writer;

        public ASMWriter(StreamWriter writer)
        {
            _writer = writer;
        }

        public void NewLine()
        {
            _writer.WriteLine();
        }

        public void Comment(string comment)
        {
            _writer.WriteLine($"\t; {comment}");
        }

        public void Label(string label)
        {
            _writer.WriteLine($"{label}:");
        }

        public void WriteCommand(ASMCommand command)
        {
            _writer.WriteLine($"\t{command}");
        }

        public void DeclareBytes(object[] bytes, string comment = "")
        {
            WriteCommand(new("db", bytes, comment));
        }

        public void DeclareNybbles(object[] nybbles, string comment = "")
        {
            WriteCommand(new("dn", nybbles, comment));
        }

        public void DeclareWords(object[] words, string comment = "")
        {
            WriteCommand(new("dw", words, comment));
        }
    }
}

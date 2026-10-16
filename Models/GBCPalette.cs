namespace CrystalLens.Models
{
    public class GBCPalette : IASMData
    {
        public readonly short[] Colors;

        public ASMFile File { get; }
        public ASMDataType Type => ASMDataType.GBCPal;

        public GBCPalette(ASMFile file, short[] colors)
        {
            File = file;
            Colors = colors;
        }

        public static (byte, byte, byte) ToRGB555(short color)
        {
            byte r = (byte)(color % (1 << 5));
            byte g = (byte)((color >> 5) % (1 << 5));
            byte b = (byte)(color >> 10);
            return (r, g, b);
        }

        public static (byte, byte, byte) ToRGB24(short color)
        {
            (byte r, byte g, byte b) = ToRGB555(color);
            return ((byte)(r * 255 / 31), (byte)(g * 255 / 31), (byte)(b * 255 / 31));
        }

        public class Serializer : ASMSerializer
        {
            private readonly int _count;

            public Serializer(int count)
            {
                _count = count;
            }

            internal override IASMData ReadAssembly(ASMReader reader, ASMFile file)
            {
                short[] colors = new short[_count];
                for (int i = 0; i < _count; i++)
                {
                    colors[i] = reader.ReadWord();
                }
                return new GBCPalette(file, colors);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                GBCPalette pal = (GBCPalette)data;
                for (int i = 0; i < _count; i++)
                {
                    short color = pal.Colors[i];
                    (byte r, byte g, byte b) = ToRGB555(color);
                    writer.WriteCommand(new("RGB", [r, g, b]));
                }
            }
        }
    }
}

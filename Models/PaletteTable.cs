namespace CrystalLens.Models
{
    public class PaletteTable : IASMData
    {
        private readonly List<(object? Normal, GBCPalette Shiny)> _palettes;

        public ASMFile File { get; }
        public ASMDataType Type => ASMDataType.Palettes;

        public PaletteTable(ASMFile file, List<(object?, GBCPalette)> palettes)
        {
            File = file;
            _palettes = palettes;
        }

        public GBCPalette GetShinyPalette(int species)
        {
            return _palettes[species].Shiny;
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
                List<(object?, GBCPalette)> palettes = [];
                for (int i = 0; i < _length; i++)
                {
                    /*
                     * The normal palette is written in pokecrystal as either two RGB instructions or an INCBIN directive.
                     * In the case of the latter, the referenced GBCPAL file only exists if the ROM has been built.
                     * Until further work is done in other areas, we will "jankily" ignore the normal palette.
                     * 
                     * The reason we can handle both cases with two reads is because all INCBIN directives here include
                     * middle_colors, and the current version of the ASMReader does not parse it.
                     * 
                     * Anyways, a whole lot of jank.
                     */
                    reader.Read();
                    reader.Read();

                    GBCPalette shiny = (GBCPalette)ASMSerializers.GBCPal.ReadAssembly(reader, file);
                    palettes.Add((null, shiny));
                }
                return new PaletteTable(file, palettes);
            }

            internal override void WriteAssembly(ASMWriter writer, IASMData data)
            {
                throw new NotImplementedException();
            }
        }
    }
}

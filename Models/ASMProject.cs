using System.Collections.ObjectModel;
using System.IO;

namespace CrystalLens.Models
{
    public class ASMProject
    {
        public static readonly Dictionary<string, byte> GenderRatioDefinitions = GenerateGenderRatioDefinitions();
        public readonly Dictionary<ASMConstantGroup, Dictionary<string, byte>> Constants = [];
        private readonly Dictionary<string, string> _pics = [];
        public string Path { get; }
        public readonly string Name;
        public ObservableCollection<ASMFile.Header> FileHeaders { get; private set; } = [];
        private PicPointerTable? _pokemonPicPointers;
        private PicPointerTable? _unownPicPointers;
        private PaletteTable? _pokemonPalettes;

        private ASMProject(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public string GetPicPath(string species, bool isBackpic = false)
        {
            PicPointerTable? selectedPicPointers = null;
            byte index;
            if (species == "UNOWN")
            {
                index = 1;
                if (_unownPicPointers == null)
                {
                    ASMFile file = ASMFile.ReadFile(new(@"data\pokemon\unown_pic_pointers.asm", ["UnownPicPointers"]), this);
                    if (file.Get("UnownPicPointers") is PicPointerTable picPointers)
                    {
                        _unownPicPointers = picPointers;
                    }
                }
                selectedPicPointers = _unownPicPointers;
            }
            else
            {
                index = Constants[ASMConstantGroup.Species][species];
                if (_pokemonPicPointers == null)
                {
                    ASMFile file = ASMFile.ReadFile(new(@"data\pokemon\pic_pointers.asm", ["PokemonPicPointers"]), this);
                    if (file.Get("PokemonPicPointers") is PicPointerTable picPointers)
                    {
                        _pokemonPicPointers = picPointers;
                    }
                }
                selectedPicPointers = _pokemonPicPointers;
            }
            string? label = isBackpic ? selectedPicPointers?.GetBackpicPointer(index) : selectedPicPointers?.GetFrontpicPointer(index);
            string? path = label != null ? _pics[label] : null;
            return path != null ? GetAbsolutePath(path) : string.Empty;
        }

        public GBCPalette GetShinyPalette(string species)
        {
            byte index = Constants[ASMConstantGroup.Species][species];
            if (_pokemonPalettes == null)
            {
                ASMFile file = ASMFile.ReadFile(new(@"data\pokemon\palettes.asm", ["PokemonPalettes"]), this);
                if (file.Get("PokemonPalettes") is PaletteTable palettes)
                {
                    _pokemonPalettes = palettes;
                }
            }
            return _pokemonPalettes.GetShinyPalette(index);
        }

        public string GetRelativePath(string absolutePath)
        {
            return System.IO.Path.GetRelativePath(Path, absolutePath);
        }

        public string GetAbsolutePath(string relativePath)
        {
            return System.IO.Path.Combine(Path, relativePath);
        }

        public static ASMProject OpenProject(string path)
        {
            ASMProject project = new(path);
            List<ASMFile.Header> headers = [];
            headers.AddRange(project.ReadFileHeadersFromDirectory(System.IO.Path.Combine(path, "data")));
            headers.AddRange(project.ReadFileHeadersFromDirectory(System.IO.Path.Combine(path, "gfx")));
            project.FileHeaders = new(headers);

            project.ReadFileHeadersFromDirectory(System.IO.Path.Combine(path, "constants")); // only for reading constants
            {
                ASMFile.Header header = ASMFile.ReadFileHeader(project.GetAbsolutePath(@"gfx\pics.asm"), project);
                ASMFile picsFile = ASMFile.ReadFile(header, project);
                foreach (string label in picsFile.Labels)
                {
                    if (picsFile.Get(label) is BinaryFileReference bin)
                    {
                        string pic = bin.Path.Replace(".2bpp.lz", ".png").Replace(".animated", "");
                        project._pics.Add(label, pic);
                    }
                }
            }

            return project;
        }

        private List<ASMFile.Header> ReadFileHeadersFromDirectory(string path)
        {
            List<ASMFile.Header> headers = [];

            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                headers.AddRange(ReadFileHeadersFromDirectory(directory));
            }

            IEnumerable<string> files = Directory.GetFiles(path).Where(file => file.EndsWith(".asm") || file.EndsWith(".pal"));
            foreach (string filePath in files)
            {
                try
                {
                    TryReadConstants(filePath);
                    if (ASMDataType.ValidPath.Invoke(System.IO.Path.GetRelativePath(Path, filePath)))
                    {
                        ASMFile.Header header = ASMFile.ReadFileHeader(filePath, this);
                        headers.Add(header);
                    }
                }
                catch (FileFormatException)
                { }
                catch (FormatException) // TEMP
                { }
                catch (InvalidOperationException) // TEMP
                { }
            }

            return headers;
        }

        private void TryReadConstants(string path)
        {
            List<Dictionary<string, byte>> constants;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            switch (fileName)
            {
                case "item_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.Item, constants[0]);
                    break;
                case "pokemon_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.Species, constants[0]);
                    break;
                case "pokemon_data_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.GrowthRate, constants[0]);
                    Constants.Add(ASMConstantGroup.EggGroup, constants[1]);
                    break;
                case "type_constants":
                    constants = ASMFile.ReadGroupedConstants(path);
                    Constants.Add(ASMConstantGroup.Type, constants[0]);
                    break;
            }
        }

        private static Dictionary<string, byte> GenerateGenderRatioDefinitions()
        {
            Dictionary<string, byte> defs = [];
            defs.Add("GENDER_F0", 0);
            defs.Add("GENDER_F12_5", 32);
            defs.Add("GENDER_F25", 63);
            defs.Add("GENDER_F50", 127);
            defs.Add("GENDER_F75", 191);
            defs.Add("GENDER_F100", 254);
            defs.Add("GENDER_UNKNOWN", 255);
            return defs;
        }
    }
}

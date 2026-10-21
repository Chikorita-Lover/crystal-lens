using System.IO;

namespace CrystalLens.Models
{
    public class ASMProject
    {
        public static readonly Dictionary<string, byte> GenderRatioDefinitions = GenerateGenderRatioDefinitions();
        private static readonly ICollection<string> LabelsToCache = ["AnimationPointers", "PokemonPicPointers", "UnownPicPointers", "PokemonPalettes"];
        public readonly Dictionary<ASMConstantGroup, Dictionary<string, byte>> Constants = [];
        public readonly Dictionary<string, ASMFile.Header> FileHeaders = [];
        private readonly Dictionary<string, ASMFile> _cachedFiles = [];
        private readonly Dictionary<string, ASMFile> _labelToCachedFile = [];
        private readonly Dictionary<string, string> _pics = [];
        public string Path { get; }
        public readonly string Name;

        private ASMProject(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
        }

        public string GetPicPath(string species, bool isBackpic = false)
        {
            PicPointerTable selectedPicPointers;
            int index;
            if (species == "UNOWN")
            {
                index = 1;
                selectedPicPointers = GetCachedData<PicPointerTable>("UnownPicPointers");
            }
            else
            {
                index = Constants[ASMConstantGroup.Species][species];
                selectedPicPointers = GetCachedData<PicPointerTable>("PokemonPicPointers");
            }
            string? label = isBackpic ? selectedPicPointers.GetBackpicPointer(index) : selectedPicPointers.GetFrontpicPointer(index);
            string? path = label != null ? _pics[label] : null;
            return path != null ? GetAbsolutePath(path) : string.Empty;
        }

        public GBCPalette GetShinyPalette(string species)
        {
            int index = Constants[ASMConstantGroup.Species][species];
            PaletteTable palettes = GetCachedData<PaletteTable>("PokemonPalettes");
            return palettes.GetShinyPalette(index);
        }

        public SpriteAnimation GetAnimation(string species)
        {
            int index = Constants[ASMConstantGroup.Species][species] - 1;
            AnimationPointerTable animPointers = GetCachedData<AnimationPointerTable>("AnimationPointers");
            string animPointer = animPointers[index];
            return GetCachedData<SpriteAnimation>(animPointer);
        }

        public string GetRelativePath(string absolutePath)
        {
            return System.IO.Path.GetRelativePath(Path, absolutePath);
        }

        public string GetAbsolutePath(string relativePath)
        {
            return System.IO.Path.Combine(Path, relativePath);
        }

        public ASMFile LoadFile(ASMFile.Header header)
        {
            if (_cachedFiles.TryGetValue(header.RelativePath, out ASMFile? file))
            {
                return file;
            }
            return ASMFile.ReadFile(header, this);
        }

        public T GetCachedData<T>(string label) where T : IASMData
        {
            ASMFile file = _labelToCachedFile[label];
            return (T)file.Get(label);
        }

        public static ASMProject OpenProject(string path)
        {
            ASMProject project = new(path);
            project.LoadFileHeadersFromDirectory(System.IO.Path.Combine(path, "constants"));
            project.LoadFileHeadersFromDirectory(System.IO.Path.Combine(path, "data"));
            project.LoadFileHeadersFromDirectory(System.IO.Path.Combine(path, "gfx"));

            project.LoadPicPaths(project.FileHeaders[@"gfx\pics.asm"]);

            return project;
        }

        private void LoadFileHeadersFromDirectory(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                LoadFileHeadersFromDirectory(directory);
            }

            IEnumerable<string> files = Directory.GetFiles(path).Where(file => file.EndsWith(".asm") || file.EndsWith(".pal"));
            foreach (string filePath in files)
            {
                try
                {
                    TryReadConstants(filePath);
                    string relativePath = GetRelativePath(filePath);
                    if (ASMDataType.ValidPath.Invoke(relativePath))
                    {
                        ASMFile.Header header = ASMFile.ReadFileHeader(filePath, this);
                        FileHeaders.Add(relativePath, header);
                        if (ShouldCacheFileByPath(relativePath) || header.Labels.Any(LabelsToCache.Contains))
                        {
                            ASMFile file = LoadFile(header);
                            _cachedFiles.Add(relativePath, file);
                            foreach (string label in header.Labels.Where(label => label.Length > 0))
                            {
                                _labelToCachedFile.Add(label, file);
                            }
                        }
                    }
                }
                catch (FileFormatException)
                { }
                catch (FormatException) // TEMP
                { }
                catch (InvalidOperationException) // TEMP
                { }
            }
        }

        private void LoadPicPaths(ASMFile.Header header)
        {
            ASMFile file = LoadFile(header);
            foreach (string label in file.Labels)
            {
                if (file.Get(label) is BinaryFileReference bin)
                {
                    string picPath = bin.Path.Replace(".2bpp.lz", ".png").Replace(".animated", "");
                    _pics.Add(label, picPath);
                }
            }
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

        private static bool ShouldCacheFileByPath(string path)
        {
            return path == @"gfx\pics.asm" || path == @"gfx\pokemon\anims.asm" || path.StartsWith(@"data\pokemon\base_stats\");
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

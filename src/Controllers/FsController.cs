using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Poe.FilesInMemory;

namespace PoeHUD.Controllers
{
    public class FsController
    {
        public readonly BaseItemTypes BaseItemTypes;
        public readonly ModsDat Mods;
        public readonly StatsDat Stats;
        public readonly TagsDat Tags;
        private readonly Dictionary<string, int> files;
        private readonly Memory mem;

        public FsController(Memory mem)
        {
            files = new Dictionary<string, int>();
            this.mem = mem;
            BaseItemTypes = new BaseItemTypes(mem, FindFile("Data/BaseItemTypes.dat"));
            Tags = new TagsDat(mem, FindFile("Data/Tags.dat"));
            Stats = new StatsDat(mem, FindFile("Data/Stats.dat"));
            Mods = new ModsDat(mem, FindFile("Data/Mods.dat"), Stats, Tags);
        }

        public int FindFile(string name)
        {
            if (!files.ContainsKey(name))
            {
                int num = mem.ReadInt(mem.AddressOfProcess + mem.offsets.FileRoot, 8);
                for (int num2 = mem.ReadInt(num); num2 != num; num2 = mem.ReadInt(num2))
                {
                    string text = mem.ReadStringU(mem.ReadInt(num2 + 8), 512);
                    if (text.Contains("."))
                    {
                        files.Add(text, mem.ReadInt(num2 + 12));
                    }
                }
            }
            return files[name];
        }
    }
}
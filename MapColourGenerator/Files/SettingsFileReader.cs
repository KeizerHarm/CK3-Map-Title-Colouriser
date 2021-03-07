using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MapColourGenerator.Files
{
    class SettingsFileReader
    {
        public static void ReadSettings(string path, ref string msg, ref bool @continue)
        {
            List<string> lines = new List<string>(File.ReadAllLines(path).Select(x => x.Split('#')[0]));
            lines.RemoveAll(x => string.IsNullOrEmpty(x));
            lines.RemoveAll(x => !x.Contains('='));
            if (lines.Count == 0)
            {
                msg += $"ERROR: No settings found!\n";
                @continue = false;
            }
            var dict = lines.ToDictionary(x => x.Split('=')[0].Trim(),
                x => x.Split('=')[1].Trim());

            Settings.SetSettings(dict, ref msg, ref @continue);
        }
    }
}

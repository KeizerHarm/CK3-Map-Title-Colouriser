using System.IO;

namespace MapColourGenerator.Files
{
    class GenericFileReader
    {
        public static string TryFindFile(string dir, string fileName, string name, ref string msg, ref bool @continue)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), dir);
            var possibleFiles = Directory.GetFiles(filePath, fileName);
            if (possibleFiles.Length < 1)
            {
                msg = $"ERROR: Could find no {name} file titled {fileName} in {filePath}!\n";
                @continue = false;
                return null;
            }
            msg = $"Found {name} file at {possibleFiles[0]}\n";
            return possibleFiles[0];
        }
    }
}

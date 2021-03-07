using MapColourGenerator.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapColourGenerator.Files
{
    class TitlesFileWriter
    {
        public static void WriteNewColoursForTitles(IEnumerable<Title> titles, string inputPath, string outputPath)
        {
            StreamReader reader = new StreamReader(File.OpenRead(inputPath));

            List<string> lines = new List<string>();
            var linesAndColours = Title.ExtractLineAndColoursFromTitles(titles);

            int depth = 0;
            int currentLineNo = 0;

            var enumerator = linesAndColours.GetEnumerator();
            enumerator.MoveNext();
            while (!reader.EndOfStream)
            {
                string originalLine = reader.ReadLine();
                string lineToWrite = "";
                depth += originalLine.Count(x => x == '{');
                depth -= originalLine.Count(x => x == '}');


                if (enumerator.Current.LineNo == currentLineNo)
                {
                    lineToWrite = new string('\t', depth);
                    lineToWrite += enumerator.Current.ColorString;

                    enumerator.MoveNext();
                }
                else
                {
                    lineToWrite = originalLine;
                }
                lines.Add(lineToWrite);
                currentLineNo++;
            }

            using (StreamWriter sw = new StreamWriter(File.Open(outputPath, FileMode.Create), new UTF8Encoding(true)))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}

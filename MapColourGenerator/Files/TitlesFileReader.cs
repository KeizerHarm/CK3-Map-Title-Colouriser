using MapColourGenerator.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MapColourGenerator.Files
{
    class TitlesFileReader
    {
        public static IEnumerable<Title> ParseInputForTitles(string filePath, ref string msg, ref bool @continue)
        {
            var titles = new List<Title>();
            StreamReader reader = new StreamReader(File.OpenRead(filePath));

            Stack<Title> titleStack = new Stack<Title>();

            int depth = 0;
            int currentTitleDepth = 0;
            int currentLineNo = 0;
            int noOfTitles = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = line.Trim();
                depth += line.Count(x => x == '{');
                depth -= line.Count(x => x == '}');

                titleStack.TryPeek(out Title thisTitle);

                if (TierExtensions.TitlePrefixes.Any(x => line.StartsWith(x))
                    || (Settings.RecolourBaronies && line.StartsWith("b_")))
                {
                    thisTitle = new Title(line.Split(' ').First());
                    noOfTitles++;
                    if (titleStack.Count() > 0)
                    {
                        titleStack.Peek().ChildTitles.Add(thisTitle);
                    }
                    else
                    {
                        titles.Add(thisTitle);
                    }

                    titleStack.Push(thisTitle);
                    currentTitleDepth = depth;
                }

                if (line.StartsWith("color") && thisTitle != null && thisTitle.R == 0)
                {
                    if(TryExtractRgb(line, out int r, out int g, out int b))
                    {
                        thisTitle.R = r;
                        thisTitle.G = g;
                        thisTitle.B = b;
                        thisTitle.ColourLineNo = currentLineNo;

                        if (line.Contains("#KEEPTHISCOLOUR"))
                        {
                            msg += $"Keeping original colour for {thisTitle.Name}\n";
                            thisTitle.KeepThisColour = true;
                        }
                    }
                }

                if (depth < currentTitleDepth)
                {
                    if (titleStack.Peek().ColourLineNo == 0)
                    {
                        msg += $"ERROR: Title {titleStack.Peek().Name} has no assigned colour!\n";
                        @continue = false;
                    }
                    titleStack.Pop();
                    currentTitleDepth--;
                }
                currentLineNo++;
            }

            msg += $"Loaded {titles.Count} top-level titles, {noOfTitles} titles in total!\n";
            return titles;
        }

        private class NamedColour
        {
            public int R;
            public int G;
            public int B;
            public string Name;

            public NamedColour(int r, int g, int b, string name)
            {
                R = r;
                G = g;
                B = b;
                Name = name;
            }
        }

        private static bool TryExtractRgb(string line, out int r, out int g, out int b)
        {
            r = 0; g = 0; b = 0;
            if(TryFindNamedColour(line, out NamedColour namedColour))
            {
                r = namedColour.R;
                g = namedColour.G;
                b = namedColour.B;
                return true;
            }

            Regex regex = new Regex(@"[0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}");
            if (regex.IsMatch(line))
            {
                var match = regex.Match(line).Value;
                var splitMatch = match.Split(' ');

                string rString = splitMatch[0];
                string gString = splitMatch[1];
                string bString = splitMatch[2];

                if (int.TryParse(rString, out r)
                 && int.TryParse(gString, out g)
                 && int.TryParse(bString, out b))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// From common\named_colors\default_colors.txt
        /// </summary>
        private readonly static NamedColour[] AllNamedColours =
        {
            new NamedColour(115, 60, 30, "brown"),
            new NamedColour(128, 128, 128, "grey"),
            new NamedColour(115, 34, 23, "red"),
            new NamedColour(20, 63, 102, "blue"),
            new NamedColour(192, 134, 48, "yellow"),
            new NamedColour(31, 77, 35, "green"),
            new NamedColour(26, 23, 19, "black"),
            new NamedColour(205, 203, 201, "white"),
            new NamedColour(90, 27, 65, "purple"),
            new NamedColour(154, 59, 0, "orange"),
            new NamedColour(42, 93, 141, "blue_light"),
            new NamedColour(51, 102, 56, "green_light"),
            new NamedColour(255, 174, 51, "yellow_light")
        };

        private static bool TryFindNamedColour(string line, out NamedColour foundNamedColour)
        {
            foundNamedColour = null;
            string lineWithoutComment = line.Split('#')[0].Trim();
            foreach(var namedColour in AllNamedColours)
            {
                if (lineWithoutComment.Contains(namedColour.Name))
                {
                    foundNamedColour = namedColour;
                    return true;
                }
            }
            return false;
        }
    }
}

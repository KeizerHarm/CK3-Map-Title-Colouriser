using MapColourGenerator.Domain;
using System;
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

                if (TierExtensions.TitlePrefixes.Any(x => line.StartsWith(x)))
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
                    Regex regex = new Regex(@"[0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}");
                    if (regex.IsMatch(line))
                    {
                        var match = regex.Match(line).Value;
                        var splitMatch = match.Split(' ');

                        string rString = splitMatch[0];
                        string gString = splitMatch[1];
                        string bString = splitMatch[2];

                        if (int.TryParse(rString, out int r)
                         && int.TryParse(gString, out int g)
                         && int.TryParse(bString, out int b))
                        {
                            thisTitle.R = r;
                            thisTitle.G = g;
                            thisTitle.B = b;
                            thisTitle.ColourLineNo = currentLineNo;
                        }

                        if (line.Contains("#KEEPTHISCOLOUR"))
                        {
                            msg += $"Keeping original colour for {thisTitle.Name}";
                            thisTitle.KeepThisColour = true;
                        }

                    }
                }

                if (depth < currentTitleDepth)
                {
                    titleStack.Pop();
                    currentTitleDepth--;
                }
                currentLineNo++;
            }

            msg += $"Loaded {titles.Count} top-level titles, {noOfTitles} titles in total!\n";
            return titles;
        }
    }
}

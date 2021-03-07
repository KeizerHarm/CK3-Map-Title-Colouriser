using Colourful;
using MapColourGenerator.Domain;
using MapColourGenerator.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MapColourGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            new Program();
            sw.Stop();
            Console.WriteLine($"That took only {sw.ElapsedMilliseconds} milliseconds!");
            Console.WriteLine("Press any key to close the terminal.");
            Console.ReadKey();
        }

        IEnumerable<Title> Titles;
        string TitleFilePath;

        public Program()
        {
            Console.WriteLine("Welcome to Keizer Harm's Map Colouring Tool!\n" +
                "I consume a landed_titles file, and return it with the titles recoloured into a nice gradient.\n" +
                "Consult the readme for more specific guidelines.\n");

            LoadSettings(out string settingsMsg, out bool @continue);
            Console.WriteLine(settingsMsg);
            if (!@continue) return;

            LoadTitles(out string titlesMsg, out @continue);
            Console.WriteLine(titlesMsg);
            if (!@continue) return;

            GenerateTitleColours(out string coloursMsg, out @continue);
            Console.WriteLine(coloursMsg);
            if (!@continue) return;

            WriteNewColoursToFile(out string colourWritingMsg, out @continue);
            Console.WriteLine(colourWritingMsg);
            if (!@continue) return;
        }

        private void LoadSettings(out string msg, out bool @continue)
        {
            msg = ""; @continue = true;
            var filePath = GenericFileReader.TryFindFile("Input", "Settings.txt", "settings", ref msg, ref @continue);
            if (!@continue) return;
            SettingsFileReader.ReadSettings(filePath, ref msg, ref @continue);
            if (!@continue) return;
            msg += $"Loaded in the following settings:\n{Settings.ToString()}\n";
        }

        private void LoadTitles(out string msg, out bool @continue)
        {
            msg = ""; @continue = true;
            TitleFilePath = GenericFileReader.TryFindFile("Input", "Titles.txt", "titles", ref msg, ref @continue);
            if (!@continue) return;
            Titles = TitlesFileReader.ParseInputForTitles(TitleFilePath, ref msg, ref @continue);
            if (!@continue) return;
            if (!Titles.Any())
            {
                msg += "ERROR: Cannot continue, no titles were found to recolour!";
                @continue = false;
            }
        }

        private void GenerateTitleColours(out string msg, out bool @continue)
        {
            msg = ""; @continue = true;

            foreach (var title in Titles)
            {
                SetColourForTitle(title, new LabColor(), null, ref msg);
                if (!@continue) return;
            }
        }

        public void SetColourForTitle(Title title, LabColor liegeColour, IEnumerable<Title> neighbourTitles, ref string msg)
        {
            neighbourTitles ??= new List<Title>();
            var neighbourColours = neighbourTitles.Select(x => x.LabColor);
            if (!title.KeepThisColour)
            {
                LabColor thisColour;
                if (title.Tier == Tier.Barony)
                {
                    thisColour = ColourStuff.GenerateNextColor_Barony(liegeColour);
                }
                else
                {
                    thisColour = ColourStuff.GenerateNextColor(liegeColour, title.Tier.GetMinDistance(), title.Tier.GetMaxDistance(), neighbourColours, title.Name, ref msg);
                }

                title.LabColor = thisColour;
            }

            List<Title> childrenWithColours = new List<Title>();
            foreach(var child in title.ChildTitles)
            {
                SetColourForTitle(child, title.LabColor, childrenWithColours, ref msg);
                childrenWithColours.Add(child);
            }
        }

        private void WriteNewColoursToFile(out string msg, out bool @continue)
        {
            msg = ""; @continue = true;
            if (!@continue) return;

            string outputPath;
            if (Settings.UseCustomOutputLocation)
            {
                outputPath = Settings.CustomOutputLocation;
            }
            else
            {
                outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Output", "RecolouredTitles.txt");
            }
            msg += $"Writing recoloured titles to {outputPath}!\n";

            TitlesFileWriter.WriteNewColoursForTitles(Titles, TitleFilePath, outputPath);
            msg += "It seems we're all done!";
        }
    }
}

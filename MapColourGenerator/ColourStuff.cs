using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using System;
using System.Collections.Generic;

namespace MapColourGenerator
{
    class ColourStuff
    {
        public static LabColor RgbToLab(int r, int g, int b)
        {
            double rOfOne = (double)r / 255;
            double gOfOne = (double)g / 255;
            double bOfOne = (double)b / 255;

            var sourceColour = new RGBColor(rOfOne, gOfOne, bOfOne);
            var converter = new ColourfulConverter();
            var colourInLab = converter.ToLab(sourceColour);
            return colourInLab;
        }

        public static (int r, int g, int b) LabToRGB(LabColor colour)
        {
            var converter = new ColourfulConverter();
            var rGBColor = converter.ToRGB(colour);
            return RgbColorToRGB(rGBColor);
        }
        public static (int r, int g, int b) RgbColorToRGB(RGBColor colour)
        {
            int r = (int)(colour.R * 255);
            int g = (int)(colour.G * 255);
            int b = (int)(colour.B * 255);
            return (r, g, b);
        }

        public static LabColor GenerateNextColor(LabColor sourceColour, double minDistance, double maxDistance, IEnumerable<LabColor> coloursToDodge, string titleName, ref string msg)
        {
            LabColor nextColour = new LabColor(100, 0, 0);
            Random r = new Random();
            bool resolved = false;
            int noOfTries = 0;
        TryAgain:
            while (!resolved && noOfTries < 5000)
            {
                noOfTries++;
                double nextL = NextDoubleFromTo(0, 100, r);
                double nextA = NextDoubleFromTo(-100, 100, r);
                double nextB = NextDoubleFromTo(-100, 100, r);
                nextColour = new LabColor(nextL, nextA, nextB);

                double distance = new CIEDE2000ColorDifference().ComputeDifference(sourceColour, nextColour);
                if (distance < minDistance || distance > maxDistance)
                {
                    goto TryAgain;
                }
                foreach (var colourToDodge in coloursToDodge)
                {
                    distance = new CIEDE2000ColorDifference().ComputeDifference(sourceColour, nextColour);
                    if (distance < minDistance)
                    {
                        goto TryAgain;
                    }
                }
                resolved = true;
            }
            if (!resolved)
            {
                msg += $"WARNING: Could not resolve colour for title {titleName}!\n";
            }
            return nextColour;
        }

        public static LabColor GenerateNextColor_Barony(LabColor sourceColour)
        {
            Random r = new Random();
            double nextL = NextDoubleFromTo(Math.Max(0, sourceColour.L - 1.5), Math.Min(sourceColour.L + 1.5, 100), r);
            double nextA = NextDoubleFromTo(Math.Max(-100, sourceColour.a - 3), Math.Min(sourceColour.a + 3, 100), r);
            double nextB = NextDoubleFromTo(Math.Max(-100, sourceColour.b - 3), Math.Min(sourceColour.b + 3, 100), r);
            return new LabColor(nextL, nextA, nextB);
        }

        private static double NextDoubleFromTo(double min, double max, Random random)
        {
            var magnitude = Math.Abs(min - max);
            var @double = random.NextDouble();
            @double *= magnitude;
            @double += min;
            return @double;
        }

        public static string ColourToString(int r, int g, int b)
        {
            return $"rgb {r} {g} {b}";
        }

        public static string ColourToString(LabColor color)
        {
            (int r, int g, int b) = LabToRGB(color);
            return ColourToString(r, g, b);
        }
    }
}

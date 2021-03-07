using Colourful;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapColourGenerator.Domain
{
    public class Title
    {
        public string Name;
        public int R;
        public int G;
        public int B;
        public int ColourLineNo;
        public Tier Tier;
        private LabColor _labColor;
        public LabColor LabColor
        {
            get
            {
                _labColor = ColourStuff.RgbToLab(R, G, B);

                return _labColor;
            }
            set
            {
                _labColor = value;
                (int r, int g, int b) = ColourStuff.LabToRGB(value);
                R = Math.Clamp(r, 1, 255);
                G = Math.Clamp(g, 1, 255);
                B = Math.Clamp(b, 1, 255);
            }
        }
        public bool KeepThisColour = false;


        public Title(string name)
        {
            Name = name;
            Tier = TierExtensions.TierFromName(name);
            if (Tier == Tier.Empire)
            {
                KeepThisColour = true;
            }
        }

        public List<Title> ChildTitles = new List<Title>();

        public override string ToString()
        {
            return $"{Name}: rgb {R} {G} {B}";
        }

        public static IOrderedEnumerable<LineAndColour> ExtractLineAndColoursFromTitles(IEnumerable<Title> titles)
        {
            List<LineAndColour> list = new List<LineAndColour>();
            foreach (var title in titles)
            {
                title.ExtractLineAndColoursFromTitles(ref list);
            }
            var orderedList = list.OrderBy(x => x.LineNo);
            return orderedList;
        }

        private void ExtractLineAndColoursFromTitles(ref List<LineAndColour> list)
        {
            list.Add(new LineAndColour(ColourLineNo, R, G, B));
            foreach (var child in ChildTitles)
            {
                child.ExtractLineAndColoursFromTitles(ref list);
            }
        }
    }
}

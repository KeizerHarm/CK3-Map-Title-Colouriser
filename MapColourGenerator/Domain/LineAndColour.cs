namespace MapColourGenerator.Domain
{
    public class LineAndColour
    {
        public string ColorString;
        public int LineNo;

        public LineAndColour(int lineNo, int r, int g, int b)
        {
            LineNo = lineNo;
            ColorString = $"color = {{ {r} {g} {b} }}";
        }
    }
}

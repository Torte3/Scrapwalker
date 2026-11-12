namespace Scrapwalker.Core.Travel.Talk
{
    public readonly struct SpokenLine
    {
        public SpokenLine(TalkLine line, RuinDefinition about, int found)
        {
            Line = line;
            About = about;
            Found = found;
        }

        public TalkLine Line { get; }
        public RuinDefinition About { get; }
        public int Found { get; }
        public AIFace Face => Line.Face;
        public string Text => Line.Text;
    }
}

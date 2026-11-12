using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    public readonly struct TutorialExplanation
    {
        public TutorialExplanation(TutorialStep? before, TutorialArea area, string text, string material)
        {
            Before = before;
            Area = area;
            Text = text;
            Material = material;
        }

        public TutorialStep? Before { get; }
        public TutorialArea Area { get; }
        public string Text { get; }
        public string Material { get; }

        public bool Precedes(TutorialStep step, MaterialDefinition making)
        {
            return (!Before.HasValue || step == Before.Value) && (Material == null || making != null && making.Key == Material);
        }
    }
}

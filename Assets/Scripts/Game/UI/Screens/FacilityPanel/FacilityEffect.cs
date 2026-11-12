namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal readonly struct FacilityEffect
    {
        public readonly string Text;
        public readonly string Note;

        public FacilityEffect(string text, string note)
        {
            Text = text;
            Note = note ?? string.Empty;
        }
    }
}
